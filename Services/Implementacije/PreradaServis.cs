using Domain.Konstante;
using Domain.Modeli;
using Domain.PomocneMetode.Biljke;
using Domain.PomocneMetode.Parfemi;
using Domain.Repozitorijumi;
using Domain.Servisi;

namespace Services.Implementacije
{
    public class PreradaServis : IPreradaServis
    {
        private readonly IBiljkeRepozitorijum _biljkeRepozitorijum;
        private readonly IParfemRepozitorijum _parfemiRepozitorijum;
        private readonly IProizvodnjaServis _proizvodnjaServis;
        private readonly ILoggerServis _logger;

        public PreradaServis(IBiljkeRepozitorijum biljkeRepozitorijum, IParfemRepozitorijum parfemiRepozitorijum, IProizvodnjaServis proizvodnjaServis, ILoggerServis logger)
        {
            _biljkeRepozitorijum = biljkeRepozitorijum;
            _parfemiRepozitorijum = parfemiRepozitorijum;
            _proizvodnjaServis = proizvodnjaServis;
            _logger = logger;
        }

        public List<Parfem> PreradiBiljke(string naziv, string tip, decimal cena, int brojBocica, int zapreminaPoBocici)
        {
            if (string.IsNullOrWhiteSpace(naziv))
            {
                _logger.LogWarning("[Prerada] Naziv parfema ne sme biti prazan.");
                return new List<Parfem>();
            }

            if (string.IsNullOrWhiteSpace(tip))
            {
                _logger.LogWarning("[Prerada] Tip parfema ne sme biti prazan.");
                return new List<Parfem>();
            }

            if (brojBocica <= 0)
            {
                _logger.LogWarning("[Prerada] Broj bočica mora biti veći od 0.");
                return new List<Parfem>();
            }
            if (!ParfemValidacija.ValidirajZapreminu(zapreminaPoBocici))
            {
                _logger.LogWarning($"[Prerada] Odbijeno: nepodržana zapremina {zapreminaPoBocici}ml.");
                return new List<Parfem>();
            }

            _logger.LogInfo($"[Prerada] Početak: naziv={naziv}, tip={tip}, brojBocica={brojBocica}, zapremina={zapreminaPoBocici}");

            int brojPotrebnihBiljaka;
            if (!BiljkaRacunanje.IzracunajPotrebanBrojBiljaka(brojBocica, zapreminaPoBocici, out brojPotrebnihBiljaka))
            {
                _logger.LogWarning("[Prerada] Neispravni parametri za računanje potrebnog broja biljaka.");
                return new List<Parfem>();
            }

            var ubrane = _biljkeRepozitorijum.VratiPoNazivuIStanji(naziv, StanjeBiljke.Ubrana, brojPotrebnihBiljaka);

            if (ubrane.Count < brojPotrebnihBiljaka)
            {
                _logger.LogWarning($"[Prerada] Nema dovoljno ubranih biljaka. Treba={brojPotrebnihBiljaka}, ima={ubrane.Count}. Pokrećem proizvodnju...");

                var primer = _biljkeRepozitorijum.NadjiPrvuPoNazivu(naziv);
                string lat = primer.Id != Guid.Empty ? primer.LatinskiNaziv : $"{naziv}_LAT";
                string zemlja = primer.Id != Guid.Empty ? primer.ZemljaPorekla : "Nepoznato";

                while (ubrane.Count < brojPotrebnihBiljaka)
                {
                    var nova = _proizvodnjaServis.PosadiNovuBiljku(naziv, lat, zemlja);
                    if (nova.Id == Guid.Empty)
                    {
                        _logger.LogError("[Prerada] Neuspešno posađena biljka (prazan objekat).");
                        return new List<Parfem>();
                    }

                    int trebaJos = brojPotrebnihBiljaka - ubrane.Count;
                    var ubranoNovo = _proizvodnjaServis.UberiBiljke(naziv, trebaJos);
                    if (ubranoNovo.Count == 0)
                    {
                        _logger.LogError("[Prerada] Neuspešna berba biljaka (vratila praznu listu).");
                        return new List<Parfem>();
                    }

                    ubrane = _biljkeRepozitorijum.VratiPoNazivuIStanji(naziv, StanjeBiljke.Ubrana, brojPotrebnihBiljaka);
                }
            }

            foreach (var biljka in ubrane)
            {
                double dodatak = BiljkaGenerator.GenerisiDodatakZaJacinu();
                biljka.JacinaAromaticnihUlja = Math.Round(biljka.JacinaAromaticnihUlja + dodatak, 2);

                if (biljka.JacinaAromaticnihUlja > PreradaKonstante.GRANICA_BALANS_ULJA)
                {
                    double procenat = Math.Round((biljka.JacinaAromaticnihUlja - PreradaKonstante.GRANICA_BALANS_ULJA) * 100.0, 2);

                    if (procenat < 0) procenat = 0;
                    if (procenat > 100) procenat = 100;

                    _logger.LogWarning($"[Prerada] Biljka prešla 4.00: id={biljka.Id}, jacina={biljka.JacinaAromaticnihUlja}. Balans procenat={procenat}%");

                    var primer = _biljkeRepozitorijum.NadjiPrvuPoNazivu(naziv);
                    string lat = primer.Id != Guid.Empty ? primer.LatinskiNaziv : $"{naziv}_LAT";
                    string zemlja = primer.Id != Guid.Empty ? primer.ZemljaPorekla : "Nepoznato";

                    var nova = _proizvodnjaServis.PosadiNovuBiljku(naziv, lat, zemlja);
                    if (nova.Id != Guid.Empty)
                    {
                        if (!_proizvodnjaServis.PromeniJacinuUlja(nova.Id, procenat))
                            _logger.LogWarning($"[Prerada] Balansiranje ulja nije uspelo za biljku id={nova.Id}");
                    }
                    else
                    {
                        _logger.LogWarning("[Prerada] Nije moguće posaditi novu biljku za balansiranje ulja.");
                    }
                }
                if (!_biljkeRepozitorijum.Izmeni(biljka))
                {
                    _logger.LogError($"[Prerada] Neuspešno čuvanje promene jačine ulja: id={biljka.Id}");
                    return new List<Parfem>();
                }
            }

            // Kreiraj parfeme
            var parfemi = new List<Parfem>();

            for (int i = 0; i < brojBocica; i++)
            {
                var biljka = ubrane[i % ubrane.Count];
                var parfem = new Parfem(naziv, tip, zapreminaPoBocici, "", cena, biljka.Id, DateTime.Now.AddYears(2));
                parfem.SerijskiBroj = $"PP-2025-{parfem.Id}";
                parfemi.Add(parfem);
            }
            // Stanje biljaka 
            for (int i = 0; i < ubrane.Count && i < brojPotrebnihBiljaka; i++)
            {
                var biljka = ubrane[i];
                biljka.Stanje = StanjeBiljke.Preradjena;

                if (!_biljkeRepozitorijum.Izmeni(biljka))
                {
                    _logger.LogError($"[Prerada] Neuspešno ažuriranje biljke: id={biljka.Id}");
                    return new List<Parfem>();
                }
            }

            foreach (var parfem in parfemi)
            {
                if (!_parfemiRepozitorijum.Dodaj(parfem))
                {
                    _logger.LogError($"[Prerada] Neuspešno dodavanje parfema: id={parfem.Id}");
                    return new List<Parfem>();
                }
            }

            _logger.LogInfo($"[Prerada] Uspešno: parfema={parfemi.Count}, utrošeno biljaka={brojPotrebnihBiljaka}");
            return parfemi;
        }
        public List<Parfem> VratiSveParfeme()
        {
            return _parfemiRepozitorijum.SviParfemi();
        }
    }
}