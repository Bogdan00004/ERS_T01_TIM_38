using Domain.Enumeracije;
using Domain.Modeli;

namespace Domain.BazaPodataka
{
    public class TabeleBazaPodataka
    {
        public List<Korisnik> Korisnici { get; set; } = new List<Korisnik>();
        // TODO: Add other database tables as needed
        public List<Biljka> Biljke { get; set; } = new List<Biljka>();
        public List<Parfem> Parfemi { get; set; } = new List<Parfem>();
        public List<Ambalaza> Ambalaze { get; set; } = new List<Ambalaza>();
        public List<Skladiste> Skladista { get; set; } = new List<Skladiste>();
        public List<FiskalniRacun> FiskalniRacuni { get; set; } = new List<FiskalniRacun>();

        public TabeleBazaPodataka() { }
        public void Seed()
        {
            Korisnici = new List<Korisnik>{
                new Korisnik(
                    korisnickoIme: "admin",
                    lozinka: "admin123",
                    imePrezime: "Bogdan Pecanac",
                    tipKorisnika: TipKorisnika.MenadzerProdaje),
                new Korisnik(
                    korisnickoIme: "prodavac",
                    lozinka: "1234567",
                imePrezime: "Petar Petrovic",
                tipKorisnika: TipKorisnika.Prodavac)};

            // ===== SKLADISTA =====
            var s1 = new Skladiste("Skladiste - Centar", "Paris (1er)", 10);
            var s2 = new Skladiste("Skladiste - Magacin", "Paris (19e)", 6);

            Skladista = new List<Skladiste> { s1, s2 };

            // ===== BILJKE =====
            var b1 = new Biljka("Lavanda", 3.20, "Lavandula angustifolia", "Francuska", StanjeBiljke.Ubrana);
            var b2 = new Biljka("Ruža", 4.10, "Rosa damascena", "Bugarska", StanjeBiljke.Ubrana);
            var b3 = new Biljka("Jasmin", 2.85, "Jasminum grandiflorum", "Indija", StanjeBiljke.Posadjena);
            var b4 = new Biljka("Bergamot", 3.95, "Citrus bergamia", "Italija", StanjeBiljke.Ubrana);

            Biljke = new List<Biljka> { b1, b2, b3, b4 };

            // ===== PARFEMI =====
            var p1 = new Parfem("Lavande Royale", "Parfem", 150, "", 120m, b1.Id, DateTime.Now.AddYears(2));
            p1.SerijskiBroj = $"PP-2025-{p1.Id}";

            var p2 = new Parfem("Lavande Royale", "Parfem", 150, "", 120m, b1.Id, DateTime.Now.AddYears(2));
            p2.SerijskiBroj = $"PP-2025-{p2.Id}";

            var p3 = new Parfem("Rose Élégante", "Kolonjska voda", 250, "", 95m, b2.Id, DateTime.Now.AddYears(2));
            p3.SerijskiBroj = $"PP-2025-{p3.Id}";

            Parfemi = new List<Parfem> { p1, p2, p3 };

            // ===== AMBALAZE =====
            var a1 = new Ambalaza("Ambalaza-001", "O'Sinjel De Or, Paris", s1.Id, StatusAmbalaze.Spakovana);
            a1.ParfemiId.Add(p1.Id);
            a1.ParfemiId.Add(p2.Id);

            var a2 = new Ambalaza("Ambalaza-002", "O'Sinjel De Or, Paris", s2.Id, StatusAmbalaze.Spakovana);
            a2.ParfemiId.Add(p3.Id);

            Ambalaze = new List<Ambalaza> { a1, a2 };

            // ===== POVEZIVANJE AMBALAZA <-> SKLADISTA =====
            s1.AmbalazeId.Add(a1.Id);
            s1.TrenutniKapacitet = 1;

            s2.AmbalazeId.Add(a2.Id);
            s2.TrenutniKapacitet = 1;

            // ===== FISKALNI RACUNI =====
            FiskalniRacuni = new List<FiskalniRacun>();
        }
    }

}


