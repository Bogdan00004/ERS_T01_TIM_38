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
            meni.PrikaziMeni();*/

            // --- TESTIRANJE SERVISA PROIZVODNJE ---

            IProizvodnjaServis proizvodnjaServis = new ProizvodnjaServis(bazaPodataka);

            // 1. Simuliramo da je neka prethodna prerađena biljka imala jačinu od 4.65
            double preradjenaJacina = 4.65;

            // 2. Sadimo novu biljku
            proizvodnjaServis.PosadiNovuBiljku("Lavanda", "Lavandula angustifolia", "Francuska");
            Console.WriteLine(" Zasađena nova biljka!");

            // 3. Uzimamo poslednju biljku iz baze (pretpostavljamo da je ta poslednja posle sadnje)
            var poslednja = bazaPodataka.Tabele.Biljke.Last();
            Console.WriteLine($"Nova biljka ima početnu jačinu: {poslednja.JacinaAromaticnihUlja}");
            // 4. Smanjujemo jačinu ako je potrebno
            proizvodnjaServis.PromeniJacinuUlja(poslednja.Id, preradjenaJacina);
            // 5. Prikazujemo novu vrednst
            var azurirana = bazaPodataka.Tabele.Biljke.FirstOrDefault(b => b.Id == poslednja.Id);
            Console.WriteLine($" Nakon smanjenja, nova jačina je: {azurirana?.JacinaAromaticnihUlja}");
            
        }
    }
}
