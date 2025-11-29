using Domain.Modeli;

namespace Domain.BazaPodataka
{
    public class TabeleBazaPodataka
    {
        public List<Korisnik> Korisnici { get; set; } = new List<Korisnik>();
        // TODO: Add other database tables as needed
        public List<Biljka> Biljke { get; set; } = new List<Biljka> (); 
        public List<Parfem> Parfemi {  get; set; }= new List<Parfem> ();
        public List<Ambalaza> Ambalaze { get; set; } = new List<Ambalaza>();

        public TabeleBazaPodataka() {
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
            Biljke = new List<Biljka> ();
        }
    }
}
