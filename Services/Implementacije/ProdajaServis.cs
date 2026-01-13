using Domain.Enumeracije;
using Domain.Modeli;
using Domain.Repozitorijumi;
using Domain.Servisi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Loger_Bloger.Servisi.Prodaja
{
    public class ProdajaServis : IProdajaServis
    {
        private readonly IParfemRepozitorijum _parfemiRepo;
        private readonly IAmbalazaRepozitorijum _ambalazeRepo;
        private readonly IFiskalniRacunRepozitorijum _racuniRepo;
        private readonly ISkladistenjeServis _skladistenjeServis;
        private readonly ILoggerServis _logger;


        public ProdajaServis(
            IParfemRepozitorijum parfemiRepo,
            IAmbalazaRepozitorijum ambalazeRepo,
            IFiskalniRacunRepozitorijum racuniRepo,
            ISkladistenjeServis skladistenjeServis,
            ILoggerServis logger)
        {
            _parfemiRepo = parfemiRepo;
            _ambalazeRepo = ambalazeRepo;
            _racuniRepo = racuniRepo;
            _skladistenjeServis = skladistenjeServis;
            _logger = logger;
        }

        // ERS-31: katalog dostupnih parfema
        public List<KatalogStavka> VratiKatalogDostupnihParfema()
        {
            var spakovaneAmbalaze = _ambalazeRepo.VratiSpakovaneAmbalaze();

            var kolicinePoParfemu = spakovaneAmbalaze
                .SelectMany(a => a.ParfemiId)
                .GroupBy(id => id)
                .ToDictionary(g => g.Key, g => g.Count());

            var sviParfemi = _parfemiRepo.SviParfemi();

            var katalog = new List<KatalogStavka>();

            foreach (var p in sviParfemi)
            {
                var raspolozivo = kolicinePoParfemu.ContainsKey(p.Id) ? kolicinePoParfemu[p.Id] : 0;
                if (raspolozivo <= 0) continue;

                katalog.Add(new KatalogStavka
                {
                    ParfemId = p.Id,
                    Naziv = p.Naziv,
                    Tip = p.Tip,
                    NetoKolicina = p.NetoKolicina,
                    Cena = p.Cena,
                    Raspolozivo = raspolozivo
                });
            }

            return katalog;
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

            var parfem = _parfemiRepo.NadjiPoId(parfemId);
            if (parfem == null)
            {
                _logger.LogError($"[Prodaja] Parfem ne postoji: parfemId={parfemId}");
                throw new Exception("Izabrani parfem ne postoji.");
            }

            var katalog = VratiKatalogDostupnihParfema();
            var stavkaKataloga = katalog.FirstOrDefault(k => k.ParfemId == parfemId);

            if (stavkaKataloga == null || stavkaKataloga.Raspolozivo < kolicinaBocica)
            {
                _logger.LogWarning($"[Prodaja] Nema dovoljno na stanju: parfemId={parfemId}, trazeno={kolicinaBocica}, raspolozivo={(stavkaKataloga == null ? 0 : stavkaKataloga.Raspolozivo)}");
                throw new Exception("Nema dovoljno parfema na stanju.");
            }

            var preuzeteAmbalaze = new List<Ambalaza>();
            int skupljenoBocica = 0;
            int pokusaji = 0;

            while (skupljenoBocica < kolicinaBocica)
            {
                pokusaji++;
                if (pokusaji > 50)
                {
                    _logger.LogError($"[Prodaja] Previše pokušaja preuzimanja ambalaža (parfemId={parfemId}).");
                    throw new Exception("Neuspeh pri preuzimanju ambalaža (previše pokušaja).");
                }

                var nove = await _skladistenjeServis.PosaljiAmbalazeProdaji(1);

                if (nove == null || nove.Count == 0)
                {
                    _logger.LogWarning($"[Prodaja] Skladište nema dostupnih spakovanih ambalaža (parfemId={parfemId}).");
                    throw new Exception("Skladište nema dostupnih spakovanih ambalaža.");
                }

                preuzeteAmbalaze.AddRange(nove);

                skupljenoBocica = preuzeteAmbalaze
                    .SelectMany(a => a.ParfemiId)
                    .Count(id => id == parfemId);
            }

            var racun = new FiskalniRacun
            {
                TipProdaje = tipProdaje,
                NacinPlacanja = nacinPlacanja
            };

            racun.Stavke.Add(new FiskalnaStavka
            {
                ParfemId = parfem.Id,
                NazivParfema = parfem.Naziv,
                Kolicina = kolicinaBocica,
                CenaPoKomadu = parfem.Cena
            });

            _racuniRepo.Dodaj(racun);
            _racuniRepo.SacuvajPromene();
            _logger.LogInfo($"[Prodaja] Prodaja uspešna: parfem={parfem.Naziv}, kolicina={kolicinaBocica}, iznos={racun.IznosZaNaplatu}");

            return racun;
        }

        // ERS-35: pregled svih računa (samo menadžer u meniju!)
        public List<FiskalniRacun> VratiSveRacune()
        {
            return _racuniRepo.VratiSve();
        }
    }
}
