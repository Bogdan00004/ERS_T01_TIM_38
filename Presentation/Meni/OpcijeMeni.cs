using Domain.Modeli;
using Domain.Servisi;
using Domain.Enumeracije;

namespace Presentation.Meni
{
    public class OpcijeMeni
    {
        private readonly Korisnik _ulogovanKorisnik;
        private readonly IProdajaServis _prodajaServis;

        public OpcijeMeni(Korisnik ulogovanKorisnik, IProdajaServis prodajaServis)
        {
            _ulogovanKorisnik = ulogovanKorisnik;
            _prodajaServis = prodajaServis;
        }

        public void PrikaziMeni()
        {
            Console.WriteLine("\n============================================ Meni ===========================================");

            bool kraj = false;
            while (!kraj)
            {
                Console.WriteLine("\n1) Prodaja");

                
                if (_ulogovanKorisnik.Uloga == TipKorisnika.MenadzerProdaje)
                    Console.WriteLine("2) Pregled svih fiskalnih računa");

                Console.WriteLine("0) Izlaz");
                Console.Write("Izbor: ");

                var izbor = Console.ReadLine();

                switch (izbor)
                {
                    case "1":
                        Console.WriteLine("Prodaja - TODO (ERS-31/32/33)");
                        break;

                    case "2":
                        if (_ulogovanKorisnik.Uloga == TipKorisnika.MenadzerProdaje)
                            PrikaziSveRacune();
                        else
                            Console.WriteLine("Nemate prava pristupa ovoj opciji.");
                        break;

                    case "0":
                        kraj = true;
                        break;

                    default:
                        Console.WriteLine("Pogrešan izbor.");
                        break;
                }
            }
        }

        private void PrikaziSveRacune()
        {
            var racuni = _prodajaServis.VratiSveRacune();

            Console.WriteLine("\n==================== SVI FISKALNI RACUNI ====================");

            if (racuni == null || racuni.Count == 0)
            {
                Console.WriteLine("Nema fiskalnih računa.");
                return;
            }

            foreach (var r in racuni)
            {
                Console.WriteLine($"ID: {r.Id}");
                Console.WriteLine($"Tip prodaje: {r.TipProdaje}");
                Console.WriteLine($"Način plaćanja: {r.NacinPlacanja}");
                Console.WriteLine($"Iznos: {r.IznosZaNaplatu}");

                Console.WriteLine("Stavke:");
                foreach (var s in r.Stavke)
                {
                    Console.WriteLine($" - {s.NazivParfema} | kom: {s.Kolicina} | cena: {s.CenaPoKomadu} | ukupno: {s.Ukupno}");
                }

                Console.WriteLine("--------------------------------------------------------------");
            }
        }
    }
}
