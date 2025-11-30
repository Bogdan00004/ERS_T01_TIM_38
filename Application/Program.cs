using Database.Implementacije;
using Database.Repozitorijumi;
using Domain.BazaPodataka;
using Domain.Enumeracije;
using Domain.Modeli;
using Domain.Repozitorijumi;
using Domain.Servisi;
using Loger_Bloger.Servisi.Skladistenje;
using Presentation.Authentifikacija;
using Presentation.Meni;
using Services.AutentifikacioniServisi;
using Services.Implementacije;
using Services.Interfaces;

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

            // Servisi
            IAutentifikacijaServis autentifikacijaServis = new AutentifikacioniServis(korisniciRepozitorijum); // TODO: Pass necessary dependencies
            // TODO: Add other necessary services

            var magacinskiServis = new MagacinskiCentarServis(skladistaRepozitorijum, ambalazaRepozitorijum);
            var distribucioniServis = new DistribucioniCentarServis(skladistaRepozitorijum, ambalazaRepozitorijum);

            ISkladistenjeServisUloge uloga = new SkladistenjeServisUloge(magacinskiServis, distribucioniServis);

            

            // Ako nema nijednog korisnika u sistemu, dodati dva nova
            if (korisniciRepozitorijum.SviKorisnici().Count() == 0)
            {
                // TODO: Add initial users to the system
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

            // Prezentacioni sloj
            Presentation.Authentifikacija.AutentifikacioniMeni am = new Presentation.Authentifikacija.AutentifikacioniMeni(autentifikacijaServis);
            Korisnik prijavljen = new Korisnik(); 
            while (am.TryLogin(out prijavljen) == false)
            {
                Console.WriteLine("Pogrešno korisničko ime ili lozinka. Pokušajte ponovo.");
            }
            ISkladistenjeServis skladistenjeServis = uloga.KreirajServis(prijavljen.Uloga);
            Console.Clear();
            Console.WriteLine($"Uspešno ste prijavljeni kao: {prijavljen.ImePrezime} ({prijavljen.Uloga})");

            OpcijeMeni meni = new OpcijeMeni(skladistenjeServis); // TODO: Pass necessary dependencies
            meni.PrikaziMeni();

        }
    }
}
