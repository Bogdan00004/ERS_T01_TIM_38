using Domain.Modeli;

namespace Domain.BazaPodataka
{
    public class TabeleBazaPodataka
    {
        public List<Korisnik> Korisnici { get; set; } = new List<Korisnik>();
        // TODO: Add other database tables as needed
         
        

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
            
        }
    }
}
