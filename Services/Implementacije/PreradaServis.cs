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
        private readonly ILoggerServis _logger;


        public PreradaServis(IBiljkeRepozitorijum biljkeRepozitorijum, IParfemRepozitorijum parfemiRepozitorijum, ILoggerServis logger)
        {
            _biljkeRepozitorijum = biljkeRepozitorijum;
            _parfemiRepozitorijum = parfemiRepozitorijum;
            _logger = logger;
        }

        public List<Parfem> PreradiBiljke(string naziv, int brojBocica, int zapreminaPoBocici)
        {
            _logger.LogInfo($"Prerada: naziv={naziv}, brojBocica={brojBocica}, zapremina={zapreminaPoBocici}");
            if (zapreminaPoBocici != 150 && zapreminaPoBocici != 250)
            {
                _logger.LogWarning($"Prerada odbijena: nepodržana zapremina {zapreminaPoBocici}");
                throw new ArgumentException("Podržane su samo zapremine od 150 ml ili 250 ml.");
            }

            int ukupnaKolicina = brojBocica * zapreminaPoBocici;
            int brojPotrebnihBiljaka = (int)Math.Ceiling((double)ukupnaKolicina / 50.0); // zaokruzujemo broj potrebnih biljaka za izradu parfemo na sledeci veci broj(za svaki slucaj)

            var dostupneBiljke = _biljkeRepozitorijum.SveBiljke().Where(b => b.Stanje == StanjeBiljke.Ubrana).Take(brojPotrebnihBiljaka).ToList();

            if (dostupneBiljke.Count < brojPotrebnihBiljaka)
            {
                _logger.LogWarning($"Prerada neuspešna: traženo biljaka={brojPotrebnihBiljaka}, dostupno={dostupneBiljke.Count}, naziv={naziv}");
                throw new InvalidOperationException("Nema dovoljno ubranih biljaka za preradu.");
            }

            var parfemi = new List<Parfem>();

            for (int i = 0; i < brojBocica; i++)
            {
                var biljka = dostupneBiljke[i % dostupneBiljke.Count];

                var parfem = new Parfem
                {
                    Id = Guid.NewGuid(),
                    Naziv = naziv,
                    NetoKolicina = zapreminaPoBocici,
                    SerijskiBroj = $"PP-2025-{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}",
                    BiljkaId = biljka.Id,
                    RokTrajanja = DateTime.Now.AddYears(2)
                };

                parfemi.Add(parfem);
            }

            // Ažuriranje stanja biljaka
            foreach (var biljka in dostupneBiljke)
            {
                biljka.Stanje = StanjeBiljke.Preradjena;
                _biljkeRepozitorijum.Izmeni(biljka);
            }

            // Dodavanje parfema u bazu
            foreach (var parfem in parfemi)
            {
                _parfemiRepozitorijum.Dodaj(parfem);
            }
            _logger.LogInfo($"Prerada uspešna: napravljeno parfema={parfemi.Count}, utrošeno biljaka={brojPotrebnihBiljaka}");
            return parfemi;
        }

        public List<Parfem> VratiSveParfeme()
        {
            return _parfemiRepozitorijum.SviParfemi();
        }
    }
}