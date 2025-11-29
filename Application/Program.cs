using Database.Implementacije;
using Database.Repozitorijumi;
using Domain.BazaPodataka;
using Domain.Modeli;
using Domain.Enumeracije;
using Domain.Repozitorijumi;
using Domain.Servisi;
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

            // Servisi
            IAutentifikacijaServis autentifikacijaServis = new AutentifikacioniServis(korisniciRepozitorijum); // TODO: Pass necessary dependencies
            // TODO: Add other necessary services

            // Ako nema nijednog korisnika u sistemu, dodati dva nova
            /*if (korisniciRepozitorijum.SviKorisnici().Count() == 0)
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

            Console.Clear();
            Console.WriteLine($"Uspešno ste prijavljeni kao: {prijavljen.ImePrezime} ({prijavljen.Uloga})");

            OpcijeMeni meni = new OpcijeMeni(); // TODO: Pass necessary dependencies
            meni.PrikaziMeni();
            */
            /*---Test primer za PreradaServis---*/
            var baza = new JsonBazaPodataka();
            var biljkeRepo = new BiljkeRepozitorijum(baza);
            var parfemiRepo = new ParfemiRepozitorijum(baza);
            var preradaServis = new PreradaServis(biljkeRepo, parfemiRepo);

            // 1. Zasadimo i oberemo biljke (da budu spremne za preradu)
            for (int i = 0; i < 5; i++)
            {
                var biljka = new Biljka
                {
                    Id = Guid.NewGuid(),
                    Naziv = $"Lavanda-{i + 1}",
                    JacinaAromaticnihUlja = 7,
                    Stanje = StanjeBiljke.Ubrana
                };
                biljkeRepo.Dodaj(biljka);
            }

            // 2. Prerada biljaka u parfeme (npr. 3 bočice po 150ml)
            var parfemi = preradaServis.PreradiBiljke("Savignon","Unisex", 3, 150);

            Console.WriteLine("Parfemi uspešno napravljeni:\n");
            foreach (var p in parfemi)
            {
                Console.WriteLine($"Naziv: {p.Naziv}, Tip: {p.Tip}, Zapremina: {p.NetoKolicina}ml, Serijski broj: {p.SerijskiBroj}");
            }

            Console.WriteLine("\nStanja biljaka nakon prerade:");
            foreach (var b in biljkeRepo.SveBiljke())
            {
                Console.WriteLine($"Biljka: {b.Naziv}, Stanje: {b.Stanje}");
            }
        }
    }
}
