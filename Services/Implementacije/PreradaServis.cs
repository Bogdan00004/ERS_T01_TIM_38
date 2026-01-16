using Domain.Servisi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Modeli;
using Domain.Repozitorijumi;

namespace Services.Implementacije
{
    public class PreradaServis : IPreradaServis
    {
        private readonly IBiljkeRepozitorijum _biljkeRepozitorijum;
        private readonly IParfemRepozitorijum _parfemiRepozitorijum;
        private readonly IProizvodnjaServis _proizvodnjaServis;
        private readonly ILoggerServis _logger;
        private static readonly Random _rng = new Random();

        public PreradaServis(IBiljkeRepozitorijum biljkeRepozitorijum, IParfemRepozitorijum parfemiRepozitorijum, IProizvodnjaServis proizvodnjaServis, ILoggerServis logger)
        {
            _biljkeRepozitorijum = biljkeRepozitorijum;
            _parfemiRepozitorijum = parfemiRepozitorijum;
            _proizvodnjaServis = proizvodnjaServis;
            _logger = logger;
        }

        public List<Parfem> PreradiBiljke(string naziv, string tip, decimal cena, int brojBocica, int zapreminaPoBocici)
        {
            if (string.IsNullOrWhiteSpace(tip))
                throw new ArgumentException("Tip parfema ne sme biti prazan.");

            _logger.LogInfo($"Prerada: naziv={naziv}, brojBocica={brojBocica}, zapremina={zapreminaPoBocici}");

            if (brojBocica <= 0)
                throw new ArgumentException("Broj bočica mora biti veći od 0.");

            if (zapreminaPoBocici != 150 && zapreminaPoBocici != 250)
            {
                _logger.LogWarning($"Prerada odbijena: nepodržana zapremina {zapreminaPoBocici}");
                throw new ArgumentException("Podržane su samo zapremine od 150 ml ili 250 ml.");
            }

            int ukupnaKolicina = brojBocica * zapreminaPoBocici;
            int brojPotrebnihBiljaka = (int)Math.Ceiling((double)ukupnaKolicina / 50.0); // zaokruzujemo broj potrebnih biljaka za izradu parfemo na sledeci veci broj(za svaki slucaj)

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
                    var noveUbrane = _proizvodnjaServis.UberiBiljke(naziv, trebaJos);

                    
                    ubrane = _biljkeRepozitorijum.SveBiljke().Where(b => b.Naziv == naziv && b.Stanje == StanjeBiljke.Ubrana).Take(brojPotrebnihBiljaka).ToList();
                }
            }

            foreach (var biljka in ubrane)
            {
                double dodatak = _rng.NextDouble(); 
                biljka.JacinaAromaticnihUlja = Math.Round(biljka.JacinaAromaticnihUlja + dodatak, 2);

                if (biljka.JacinaAromaticnihUlja > 4.00)
                {
                   
                    double odstupanje = biljka.JacinaAromaticnihUlja - 4.00;  // npr 0.65
                    double procenat = Math.Round(odstupanje * 100.0, 2);       // 65%

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