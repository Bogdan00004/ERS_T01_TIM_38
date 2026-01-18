using Domain.Servisi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Modeli;
using Domain.Repozitorijumi;
using Domain.Konstante;
using Domain.PomocneMetode.Biljke;
using Domain.PomocneMetode.Parfemi;


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
                throw new ArgumentException("Naziv parfema ne sme biti prazan.");

            if (string.IsNullOrWhiteSpace(tip))
                throw new ArgumentException("Tip parfema ne sme biti prazan.");

            _logger.LogInfo($"Prerada: naziv={naziv}, brojBocica={brojBocica}, zapremina={zapreminaPoBocici}");

            if (brojBocica <= 0)
                throw new ArgumentException("Broj bočica mora biti veći od 0.");

            try
            {
                ParfemValidacija.ValidirajZapreminu(zapreminaPoBocici);
            }
            catch (ArgumentException)
            {
                _logger.LogWarning($"Prerada odbijena: nepodržana zapremina {zapreminaPoBocici}");
                throw;
            }

            int brojPotrebnihBiljaka = BiljkaRacunanje.IzracunajPotrebneBiljke(brojBocica, zapreminaPoBocici);

            var ubrane = _biljkeRepozitorijum.SveBiljke()
                .Where(b => b.Naziv == naziv && b.Stanje == StanjeBiljke.Ubrana)
                .Take(brojPotrebnihBiljaka)
                .ToList();

            
            if (ubrane.Count < brojPotrebnihBiljaka)
            {
                _logger.LogWarning($"[Prerada] Nema dovoljno ubranih biljaka. Treba={brojPotrebnihBiljaka}, ima={ubrane.Count}. Pokrećem proizvodnju...");

                var primer = _biljkeRepozitorijum.SveBiljke().FirstOrDefault(b => b.Naziv == naziv);
                string lat = primer?.LatinskiNaziv ?? $"{naziv}_LAT";
                string zemlja = primer?.ZemljaPorekla ?? "Nepoznato";

                while (ubrane.Count < brojPotrebnihBiljaka)
                {
                    _proizvodnjaServis.PosadiNovuBiljku(naziv, lat, zemlja);

                    int trebaJos = brojPotrebnihBiljaka - ubrane.Count;
                    _proizvodnjaServis.UberiBiljke(naziv, trebaJos);
                    ubrane = _biljkeRepozitorijum.SveBiljke().Where(b => b.Naziv == naziv && b.Stanje == StanjeBiljke.Ubrana).Take(brojPotrebnihBiljaka).ToList();
                }
            }

            foreach (var biljka in ubrane)
            {
                double dodatak = BiljkaGenerator.GenerisiDodatakZaJacinu();
                biljka.JacinaAromaticnihUlja = Math.Round(biljka.JacinaAromaticnihUlja + dodatak, 2);

                if (biljka.JacinaAromaticnihUlja > PreradaKonstante.GRANICA_BALANS_ULJA)
                {
                   
                    double procenat = Math.Round((PreradaKonstante.GRANICA_BALANS_ULJA / biljka.JacinaAromaticnihUlja) * 100.0, 2);  

                    _logger.LogWarning($"[Prerada] Biljka prešla 4.00: id={biljka.Id}, jacina={biljka.JacinaAromaticnihUlja}. Balans procenat={procenat}%");

                    var primer = _biljkeRepozitorijum.SveBiljke().FirstOrDefault(b => b.Naziv == naziv);
                    string lat = primer?.LatinskiNaziv ?? $"{naziv}_LAT";
                    string zemlja = primer?.ZemljaPorekla ?? "Nepoznato";

                    var nova = _proizvodnjaServis.PosadiNovuBiljku(naziv, lat, zemlja);
                    _proizvodnjaServis.PromeniJacinuUlja(nova.Id, procenat);
                }
            }

            // Kreiraj parfeme
            var parfemi = new List<Parfem>();

            for (int i = 0; i < brojBocica; i++)
            {
                var biljka = ubrane[i % ubrane.Count];

                var parfem = new Parfem
                {
                    Id = Guid.NewGuid(),
                    Naziv = naziv,
                    Tip = tip,
                    NetoKolicina = zapreminaPoBocici,
                    BiljkaId = biljka.Id,
                    RokTrajanja = DateTime.Now.AddYears(2),
                    Cena = cena
                };

                parfem.SerijskiBroj = $"PP-2025-{parfem.Id}";

                parfemi.Add(parfem);
            }

            // Stanje biljaka 
            var utrosene = ubrane.Take(brojPotrebnihBiljaka).ToList();
            foreach (var biljka in utrosene)
            {
                biljka.Stanje = StanjeBiljke.Preradjena;
                _biljkeRepozitorijum.Izmeni(biljka);
            }

            // Upis parfema u repo
            foreach (var parfem in parfemi)
                _parfemiRepozitorijum.Dodaj(parfem);

            _logger.LogInfo($"[Prerada] Uspešno: parfema={parfemi.Count}, utrošeno biljaka={brojPotrebnihBiljaka}");
            return parfemi;
        }

        public List<Parfem> VratiSveParfeme()
        {
            return _parfemiRepozitorijum.SviParfemi();
        }
    }
}