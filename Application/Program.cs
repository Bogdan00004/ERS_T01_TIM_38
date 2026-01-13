using Database.Implementacije;
using Database.Repozitorijumi;
using Domain.BazaPodataka;
using Domain.Enumeracije;
using Domain.Modeli;
using Domain.Repozitorijumi;
using Domain.Servisi;
using Loger_Bloger.Servisi.Skladistenje;
using Presentation.Meni;
using Services.AutentifikacioniServisi;
using Services.LoggerServisi;

namespace Loger_Bloger
{
    public class Program
    {
        public static void Main()
        {
            // Baza podataka
            IBazaPodataka bazaPodataka = new JsonBazaPodataka();

            // Repozitorijumi
            IKorisniciRepozitorijum korisniciRepozitorijum = new KorisniciRepozitorijum(bazaPodataka);
            ISkladistaRepozitorijum skladistaRepozitorijum = new SkladistaRepozitorijum(bazaPodataka);
            IAmbalazaRepozitorijum ambalazaRepozitorijum = new AmbalazeRepozitorijum(bazaPodataka);

            // DODATO: parfemi + racuni
            IParfemRepozitorijum parfemRepozitorijum = new ParfemiRepozitorijum(bazaPodataka);
            IFiskalniRacunRepozitorijum fiskalniRacunRepozitorijum = new FiskalniRacuniRepozitorijum(bazaPodataka);
            ILoggerServis logger = new TekstualniLoggerServis("log.txt");

            // Servisi
            IAutentifikacijaServis autentifikacijaServis = new AutentifikacioniServis(korisniciRepozitorijum,logger);

            var magacinskiServis = new MagacinskiCentarServis(skladistaRepozitorijum, ambalazaRepozitorijum,logger);
            var distribucioniServis = new DistribucioniCentarServis(skladistaRepozitorijum, ambalazaRepozitorijum, logger);
            ISkladistenjeServisUloge uloga = new SkladistenjeServisUloge(magacinskiServis, distribucioniServis);

            // Seed korisnika
            if (korisniciRepozitorijum.SviKorisnici().Count() == 0)
            {
                korisniciRepozitorijum.DodajKorisnika(new Korisnik
                {
                    KorisnickoIme = "Bogdan",
                    Lozinka = "123",
                    ImePrezime = "Bogdan Pecanac",
                    Uloga = TipKorisnika.MenadzerProdaje
                });

                korisniciRepozitorijum.DodajKorisnika(new Korisnik
                {
                    KorisnickoIme = "Petar",
                    Lozinka = "321",
                    ImePrezime = "Petar Petrovic",
                    Uloga = TipKorisnika.Prodavac
                });
            }

            // Login
            var am = new Presentation.Authentifikacija.AutentifikacioniMeni(autentifikacijaServis);
            Korisnik prijavljen = new Korisnik();

            while (am.TryLogin(out prijavljen) == false)
            {
                Console.WriteLine("Pogrešno korisničko ime ili lozinka. Pokušajte ponovo.");
            }

            // Kreiraj skladistenje servis po ulozi
            ISkladistenjeServis skladistenjeServis = uloga.KreirajServis(prijavljen.Uloga);

            
            IProdajaServis prodajaServis = new Loger_Bloger.Servisi.Prodaja.ProdajaServis(
                parfemRepozitorijum,
                ambalazaRepozitorijum,
                fiskalniRacunRepozitorijum,
                skladistenjeServis,logger);

            Console.Clear();
            Console.WriteLine($"Uspešno ste prijavljeni kao: {prijavljen.ImePrezime} ({prijavljen.Uloga})");

            
            OpcijeMeni meni = new OpcijeMeni(prijavljen, prodajaServis);
            meni.PrikaziMeni();
        }
    }
}
