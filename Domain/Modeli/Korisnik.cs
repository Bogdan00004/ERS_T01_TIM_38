using Domain.Enumeracije;

namespace Domain.Modeli
{
    public class Korisnik
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string KorisnickoIme { get; set; } = string.Empty;
        public string Lozinka { get; set; } = string.Empty;
        public string ImePrezime { get; set; } = string.Empty;
        public TipKorisnika Uloga { get; set; }

        public Korisnik() {
            Id = Guid.Empty;
        }

        public Korisnik(string korisnickoIme, string lozinka, string imePrezime, TipKorisnika tipKorisnika)
        {
            KorisnickoIme = korisnickoIme;
            Lozinka = lozinka;
            ImePrezime = imePrezime;
            Uloga = tipKorisnika;
        }
    }
}
