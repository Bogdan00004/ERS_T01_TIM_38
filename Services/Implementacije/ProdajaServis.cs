using Domain.Enumeracije;
using Domain.Modeli;
using Domain.PomocneMetode.Prodaja;
using Domain.Repozitorijumi;
using Domain.Servisi;

namespace Loger_Bloger.Servisi.Prodaja
{
    public class ProdajaServis : IProdajaServis
    {
        private readonly IParfemRepozitorijum _parfemiRepo;
        private readonly IAmbalazaRepozitorijum _ambalazeRepo;
        private readonly IFiskalniRacunRepozitorijum _racuniRepo;
        private readonly ISkladistenjeServis _skladistenjeServis;
        private readonly IPakovanjeServis _pakovanjeServis;
        private readonly ISkladistaRepozitorijum _skladistaRepo;
        private readonly ILoggerServis _logger;


        public ProdajaServis(IParfemRepozitorijum parfemiRepo, IAmbalazaRepozitorijum ambalazeRepo, IFiskalniRacunRepozitorijum racuniRepo, ISkladistenjeServis skladistenjeServis, IPakovanjeServis pakovanjeServis, ISkladistaRepozitorijum skladistaRepo, ILoggerServis logger)
        {
            _parfemiRepo = parfemiRepo;
            _ambalazeRepo = ambalazeRepo;
            _racuniRepo = racuniRepo;
            _skladistenjeServis = skladistenjeServis;
            _pakovanjeServis = pakovanjeServis;
            _skladistaRepo = skladistaRepo;
            _logger = logger;
        }

        private static string NapraviKljuc(Parfem p)
        {
            var cenaNorm = decimal.Round(p.Cena, 2);
            return $"{p.Naziv}|{p.Tip}|{p.NetoKolicina}|{cenaNorm:0.##}";

        }

        //katalog dostupnih parfema
        public List<KatalogStavka> VratiKatalogDostupnihParfema()
        {
            // Uzimamo sva skladišta i pravimo skup ambalaža koje su fizički u skladištu
            var skladista = _skladistaRepo.VratiSva();

            // Katalog računa samo spakovane ambalaže koje su u skladištu
            var spakovaneAmbalaze = _ambalazeRepo.VratiSpakovaneAmbalaze().Where(a => skladista.Any(s => s.AmbalazeId.Contains(a.Id))).ToList();

            // mapa ID->parfem da bismo mogli iz ambalaze (koja cuva samo ID) da izvucemo detalje
            var sviParfemi = _parfemiRepo.SviParfemi();
            var mapa = sviParfemi.ToDictionary(p => p.Id, p => p);

            // brojimo raspolozive bocice po "proizvodu" (naziv+tip+zapremina+cena)
            var kolicine = new Dictionary<string, int>();
            var predstavnik = new Dictionary<string, Parfem>();

            foreach (var amb in spakovaneAmbalaze)
            {
                foreach (var parfemId in amb.ParfemiId)
                {
                    if (!mapa.ContainsKey(parfemId)) continue;

                    var p = mapa[parfemId];
                    var key = NapraviKljuc(p);

                    if (!kolicine.ContainsKey(key)) kolicine[key] = 0;
                    kolicine[key]++;

                    if (!predstavnik.ContainsKey(key)) predstavnik[key] = p;
                }
            }

            var katalog = new List<KatalogStavka>();
            foreach (var kv in kolicine)
            {
                var p = predstavnik[kv.Key];
                katalog.Add(new KatalogStavka(p.Id, p.Naziv, p.Tip, p.NetoKolicina, p.Cena, kv.Value));
            }

            return katalog.OrderBy(k => k.Naziv).ThenBy(k => k.Tip).ThenBy(k => k.NetoKolicina).ToList();
        }


        // ERS-33: prodaja -> traži skladište -> raspakuje -> doda u račun
        public async Task<FiskalniRacun> Prodaj(Guid parfemId, int kolicinaBocica, TipProdaje tipProdaje, NacinPlacanja nacinPlacanja)
        {
            _logger.LogInfo($"[Prodaja] Početak prodaje: parfemId={parfemId}, kolicina={kolicinaBocica}, tip={tipProdaje}, nacin={nacinPlacanja}");

            if (kolicinaBocica <= 0)
            {
                _logger.LogWarning($"[Prodaja] Neispravna količina: {kolicinaBocica}");
                throw new ArgumentException("Količina bočica mora biti veća od 0.");
            }

            var izabrani = _parfemiRepo.NadjiPoId(parfemId);
            if (izabrani.Id == Guid.Empty)
            {
                _logger.LogError($"[Prodaja] Parfem ne postoji: parfemId={parfemId}");
                throw new Exception("Izabrani parfem ne postoji.");
            }

            var katalog = VratiKatalogDostupnihParfema();
            var stavkaKataloga = KatalogHelper.NadjiStavku(katalog, izabrani);
            int raspolozivo = stavkaKataloga?.Raspolozivo ?? 0;

            if (raspolozivo < kolicinaBocica)
            {
                int nedostaje = kolicinaBocica - raspolozivo;

                _logger.LogWarning($"[Prodaja] Nema dovoljno na stanju. Raspolozivo={raspolozivo}, trazeno={kolicinaBocica}, nedostaje={nedostaje}. Pokrecem pakovanje...");

                // izaberi skladiste koje ima kapacitet
                var skladista = _skladistaRepo.VratiSva();
                var ciljnoSkladiste = skladista
                    .Where(s => s.TrenutniKapacitet < s.MaxKapacitet)
                    .OrderByDescending(s => (s.MaxKapacitet - s.TrenutniKapacitet))
                    .FirstOrDefault();

                if (ciljnoSkladiste == null)
                    throw new Exception("Nema skladišta sa slobodnim kapacitetom.");

                // Obezbedi ambalazu u skladistu: ako nema -> spakuj -> ubaci
                _pakovanjeServis.ObezbediAmbalazuUSkladistu(
                    nazivParfema: izabrani.Naziv,
                    tip: izabrani.Tip,
                    cena: izabrani.Cena,
                    brojBocica: nedostaje,
                    zapreminaPoBocici: izabrani.NetoKolicina,
                    skladisteId: ciljnoSkladiste.Id
                );

                // osvezi katalog posle pakovanja
                katalog = VratiKatalogDostupnihParfema();
                stavkaKataloga = KatalogHelper.NadjiStavku(katalog, izabrani);

                if (stavkaKataloga == null || stavkaKataloga.Raspolozivo < kolicinaBocica)
                    throw new Exception("Nije moguće obezbediti dovoljno parfema ni nakon pakovanja.");
            }


            var preuzeteAmbalaze = new List<Ambalaza>();
            int skupljenoBocica = 0;
            int pokusaji = 0;

            var sviParfemi = _parfemiRepo.SviParfemi();
            var mapa = sviParfemi.ToDictionary(p => p.Id, p => p);

            string trazeniKey = NapraviKljuc(izabrani);

            while (skupljenoBocica < kolicinaBocica)
            {
                pokusaji++;
                if (pokusaji > 50)
                {
                    _logger.LogError($"[Prodaja] Previše pokušaja preuzimanja ambalaža (parfemId={parfemId}).");
                    throw new Exception("Neuspeh pri preuzimanju ambalaža (previše pokušaja).");
                }

                int josTreba = kolicinaBocica - skupljenoBocica;
                int trazeneAmbalaze = Math.Min(josTreba, 3);
                var nove = await _skladistenjeServis.PosaljiAmbalazeProdaji(trazeneAmbalaze);

                if (nove == null || nove.Count == 0)
                {
                    _logger.LogWarning($"[Prodaja] Skladište nema dostupnih spakovanih ambalaža (parfemId={parfemId}).");
                    throw new Exception("Skladište nema dostupnih spakovanih ambalaža.");
                }

                preuzeteAmbalaze.AddRange(nove);

                skupljenoBocica = preuzeteAmbalaze
                    .SelectMany(a => a.ParfemiId)
                    .Where(id => mapa.ContainsKey(id))
                    .Select(id => mapa[id])
                    .Count(p => NapraviKljuc(p) == trazeniKey);
            }
            int preostaloZaSkidanje = kolicinaBocica;
            foreach (var amb in preuzeteAmbalaze)
            {
                if (preostaloZaSkidanje <= 0) break;

                var odgovarajuci = amb.ParfemiId
                    .Where(id => mapa.ContainsKey(id) && NapraviKljuc(mapa[id]) == trazeniKey)
                    .Take(preostaloZaSkidanje)
                    .ToList();

                foreach (var id in odgovarajuci)
                    amb.ParfemiId.Remove(id);

                preostaloZaSkidanje -= odgovarajuci.Count;
            }
            foreach (var amb in preuzeteAmbalaze)
            {
                if (!_ambalazeRepo.Izmeni(amb))
                    throw new Exception("Neuspešno čuvanje promena ambalaža (skidanje parfema).");
            }

            foreach (var amb in preuzeteAmbalaze)
            {
                // Ako ima jos parfema u ambalazi -> vrati je u skladiste kao SPAKOVANU
                if (amb.ParfemiId.Count > 0)
                {
                    var skladiste = _skladistaRepo.NadjiPoId(amb.SkladisteId);

                    if (skladiste.Id != Guid.Empty)
                    {
                        // vracamo je nazad u skladiste (posto je ranije skinuta pri slanju prodaji)
                        if (!skladiste.AmbalazeId.Contains(amb.Id))
                        {
                            if (skladiste.TrenutniKapacitet < skladiste.MaxKapacitet)
                            {
                                skladiste.AmbalazeId.Add(amb.Id);
                                skladiste.TrenutniKapacitet++;
                            }
                            else
                            {
                                _logger.LogWarning($"[Prodaja] Ne mogu da vratim ambalazu u skladiste (popunjeno). ambalazaId={amb.Id}, skladisteId={skladiste.Id}");
                            }
                        }

                        if (!_skladistaRepo.Izmeni(skladiste))
                            throw new Exception("Neuspešno čuvanje promena skladišta.");
                    }

                    // vracamo status na Spakovana da bi je katalog opet video
                    amb.Status = StatusAmbalaze.Spakovana;
                    _logger.LogInfo($"[Prodaja] Ambalaza vracena u skladiste (ima ostatak parfema). ambalazaId={amb.Id}, preostaloParfema={amb.ParfemiId.Count}");
                }
                else
                {
                    amb.Status = StatusAmbalaze.Poslata;
                }
            }
           
            foreach (var amb in preuzeteAmbalaze)
            {
                if (!_ambalazeRepo.Izmeni(amb))
                    throw new Exception("Neuspešno čuvanje promena ambalaža (status).");
            }

            var racun = new FiskalniRacun(tipProdaje, nacinPlacanja);

            racun.Stavke.Add(new FiskalnaStavka(izabrani.Id, $"{izabrani.Naziv} ({izabrani.Tip}, {izabrani.NetoKolicina}ml)", kolicinaBocica, izabrani.Cena));

            if (!_racuniRepo.Dodaj(racun))
                throw new Exception("Neuspešno čuvanje fiskalnog računa.");
            _logger.LogInfo($"[Prodaja] Prodaja uspešna: parfem={izabrani.Naziv}, kolicina={kolicinaBocica}, iznos={racun.IznosZaNaplatu}");

            return racun;
        }

        // ERS-35: pregled svih računa (samo menadžer u meniju!)
        public List<FiskalniRacun> VratiSveRacune()
        {
            return _racuniRepo.VratiSve();
        }
    }
}
