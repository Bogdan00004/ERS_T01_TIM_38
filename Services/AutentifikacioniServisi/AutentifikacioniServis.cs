using Domain.Modeli;
using Domain.Servisi;
using Domain.Repozitorijumi;

namespace Services.AutentifikacioniServisi
{
    public class AutentifikacioniServis : IAutentifikacijaServis
    {
        // TODO: Add necessary dependencies (e.g., user repository) via dependency injection

        private readonly IKorisniciRepozitorijum korisnici;
        private readonly ILoggerServis _logger;

        public AutentifikacioniServis(IKorisniciRepozitorijum korisniciRepozitorijum, ILoggerServis logger)
        {
            korisnici = korisniciRepozitorijum;
            _logger = logger;
        }

        public (bool, Korisnik) Prijava(string korisnickoIme, string lozinka)
        {
            // TODO: Implement login method
            var korisnik = korisnici.PronadjiKorisnikaPoKorisnickomImenu(korisnickoIme);

            if (korisnik != null && korisnik.Lozinka == lozinka)
            {
                _logger.LogInfo($"Prijava uspešna: {korisnickoIme}");
                return (true, korisnik);
            }
            _logger.LogWarning($"Neuspešna prijava: {korisnickoIme}");
            return (false, new Korisnik());  
        }
        public (bool,Korisnik) Registracija(Korisnik noviKorisnik)
        {
            _logger.LogInfo($"Pokušaj registracije: {noviKorisnik.KorisnickoIme}");
            var postoji = korisnici.PronadjiKorisnikaPoKorisnickomImenu(noviKorisnik.KorisnickoIme);

            if (postoji != null && postoji.KorisnickoIme != string.Empty)
            {
                _logger.LogWarning($"Registracija odbijena (zauzeto): {noviKorisnik.KorisnickoIme}");
                return (false, new Korisnik());
            }

            var dodati = korisnici.DodajKorisnika(noviKorisnik);
            _logger.LogInfo($"Registracija uspešna: {noviKorisnik.KorisnickoIme}");
            return (true, dodati);
        }
    }
}
