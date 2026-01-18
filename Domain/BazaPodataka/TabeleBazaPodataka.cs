using Domain.Enumeracije;
using Domain.Modeli;

namespace Domain.BazaPodataka
{
    public class TabeleBazaPodataka
    {
        public List<Korisnik> Korisnici { get; set; } = new List<Korisnik>();
        // TODO: Add other database tables as needed
        public List<Biljka> Biljke { get; set; } = new List<Biljka>(); 
        public List<Parfem> Parfemi { get; set; }= new List<Parfem>();
        public List<Ambalaza> Ambalaze { get; set; } = new List<Ambalaza>();
        public List<Skladiste> Skladista { get; set; }=new List<Skladiste>();
        public List<FiskalniRacun> FiskalniRacuni { get; set; } = new List<FiskalniRacun>();

        public TabeleBazaPodataka() { }
        public void Seed() {
            Korisnici = new List<Korisnik>
            {
                new Korisnik(
                    korisnickoIme: "admin",
                    lozinka: "admin123",
                    imePrezime:"Bogdan Pecanac",
                    tipKorisnika: Enumeracije.TipKorisnika.MenadzerProdaje
                    ),
                new Korisnik(
                        korisnickoIme: "prodavac",
                        lozinka: "1234567",
                        imePrezime: "Petar Petrovic",
                        tipKorisnika: Enumeracije.TipKorisnika.Prodavac
                    )
            };
            var s1 = new Skladiste
            {
                Naziv = "Skladiste - Centar",
                Lokacija = "Paris (1er)",
                MaxKapacitet = 10
            };

            var s2 = new Skladiste
            {
                Naziv = "Skladiste - Magacin",
                Lokacija = "Paris (19e)",
                MaxKapacitet = 6
            };

            Skladista = new List<Skladiste> { s1, s2 };

            // ===== BILJKE =====
            var b1 = new Biljka { Naziv = "Lavanda", LatinskiNaziv = "Lavandula angustifolia", ZemljaPorekla = "Francuska", JacinaAromaticnihUlja = 3.20, Stanje = StanjeBiljke.Ubrana };
            var b2 = new Biljka { Naziv = "Ruža", LatinskiNaziv = "Rosa damascena", ZemljaPorekla = "Bugarska", JacinaAromaticnihUlja = 4.10, Stanje = StanjeBiljke.Ubrana };
            var b3 = new Biljka { Naziv = "Jasmin", LatinskiNaziv = "Jasminum grandiflorum", ZemljaPorekla = "Indija", JacinaAromaticnihUlja = 2.85, Stanje = StanjeBiljke.Posadjena };
            var b4 = new Biljka { Naziv = "Bergamot", LatinskiNaziv = "Citrus bergamia", ZemljaPorekla = "Italija", JacinaAromaticnihUlja = 3.95, Stanje = StanjeBiljke.Ubrana };

            Biljke = new List<Biljka> { b1, b2, b3, b4 };

            // ===== PARFEMI + AMBALAZE  =====
            
            var p1 = new Parfem { Id = Guid.NewGuid(), Naziv = "Lavande Royale", Tip = "Parfem", NetoKolicina = 150, Cena = 120m, BiljkaId = b1.Id, RokTrajanja = DateTime.Now.AddYears(2) };
            p1.SerijskiBroj = $"PP-2025-{p1.Id}";

            var p2 = new Parfem { Id = Guid.NewGuid(), Naziv = "Lavande Royale", Tip = "Parfem", NetoKolicina = 150, Cena = 120m, BiljkaId = b1.Id, RokTrajanja = DateTime.Now.AddYears(2) };
            p2.SerijskiBroj = $"PP-2025-{p2.Id}";

            var p3 = new Parfem { Id = Guid.NewGuid(), Naziv = "Rose Élégante", Tip = "Kolonjska voda", NetoKolicina = 250, Cena = 95m, BiljkaId = b2.Id, RokTrajanja = DateTime.Now.AddYears(2) };
            p3.SerijskiBroj = $"PP-2025-{p3.Id}";

            Parfemi = new List<Parfem> { p1, p2, p3 };

            var a1 = new Ambalaza
            {
                Naziv = "Ambalaza-001",
                AdresaPosiljaoca = "O'Sinjel De Or, Paris",
                SkladisteId = s1.Id,
                Status = StatusAmbalaze.Spakovana
            };
            a1.ParfemiId.Add(p1.Id);
            a1.ParfemiId.Add(p2.Id);

            var a2 = new Ambalaza
            {
                Naziv = "Ambalaza-002",
                AdresaPosiljaoca = "O'Sinjel De Or, Paris",
                SkladisteId = s2.Id,
                Status = StatusAmbalaze.Spakovana
            };
            a2.ParfemiId.Add(p3.Id);

            Ambalaze = new List<Ambalaza> { a1, a2 };

            
            s1.AmbalazeId.Add(a1.Id);
            s1.TrenutniKapacitet = 1;

            s2.AmbalazeId.Add(a2.Id);
            s2.TrenutniKapacitet = 1;

            // ===== FISKALNI RACUNI =====
            FiskalniRacuni = new List<FiskalniRacun>();
        }
    }

}
    

