using Domain.Modeli;
using Domain.Servisi;
using Domain.Repozitorijumi;

namespace Services.AutentifikacioniServisi
{
    public class AutentifikacioniServis : IAutentifikacijaServis
    {
        // TODO: Add necessary dependencies (e.g., user repository) via dependency injection

        private readonly IKorisniciRepozitorijum korisnici;

        public AutentifikacioniServis(IKorisniciRepozitorijum korisniciRepozitorijum)
        {
            korisnici = korisniciRepozitorijum;
        }

        public (bool, Korisnik) Prijava(string korisnickoIme, string lozinka)
        {
            // TODO: Implement login method
            var korisnik = korisnici.PronadjiKorisnikaPoKorisnickomImenu(korisnickoIme);

            if(korisnik != null && korisnik.Lozinka == lozinka)
                return(true, korisnik);
          
            return(false, new Korisnik());  
        }
        public (bool,Korisnik) Registracija(Korisnik noviKorisnik)
        {
            var postoji = korisnici.PronadjiKorisnikaPoKorisnickomImenu(noviKorisnik.KorisnickoIme);

            if (postoji != null && postoji.KorisnickoIme != string.Empty)
                return (false, new Korisnik());

            korisnici.DodajKorisnika(noviKorisnik);
            return(true, new Korisnik());
        }
    }
}
