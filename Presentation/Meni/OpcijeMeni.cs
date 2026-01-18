using Domain.Enumeracije;
using Domain.Modeli;
using Domain.Servisi;

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
                        IzvrsiProdaju();
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
        private void IzvrsiProdaju()
        {
            var katalog = _prodajaServis.VratiKatalogDostupnihParfema();

            Console.WriteLine("\n==================== KATALOG DOSTUPNIH PARFEMA ====================");
            if (katalog == null || katalog.Count == 0)
            {
                Console.WriteLine("Trenutno nema dostupnih parfema na stanju.");
                return;
            }

            for (int i = 0; i < katalog.Count; i++)
            {
                var k = katalog[i];
                Console.WriteLine($"{i + 1}) {k.Naziv} | {k.Tip} | {k.NetoKolicina}ml | cena={k.Cena:0.##} | raspolozivo={k.Raspolozivo}");
            }

            Console.Write("\nIzaberite redni broj parfema: ");
            if (!int.TryParse(Console.ReadLine(), out int rb) || rb < 1 || rb > katalog.Count)
            {
                Console.WriteLine("Neispravan izbor.");
                return;
            }

            var izabrano = katalog[rb - 1];

            Console.Write("Unesite količinu bočica: ");
            if (!int.TryParse(Console.ReadLine(), out int kolicina) || kolicina <= 0)
            {
                Console.WriteLine("Neispravna količina.");
                return;
            }

            Console.WriteLine("\nTip prodaje:");
            Console.WriteLine("1) Maloprodaja");
            Console.WriteLine("2) Veleprodaja");
            Console.Write("Izbor: ");
            var tp = Console.ReadLine();
            TipProdaje tipProdaje = (tp == "2") ? TipProdaje.Veleprodaja : TipProdaje.Maloprodaja;

            Console.WriteLine("\nNačin plaćanja:");
            Console.WriteLine("1) Gotovina");
            Console.WriteLine("2) Uplata na račun");
            Console.WriteLine("3) Kartično plaćanje");
            Console.Write("Izbor: ");
            var np = Console.ReadLine();

            NacinPlacanja nacinPlacanja = np switch
            {
                "2" => NacinPlacanja.UplataNaRacun,
                "3" => NacinPlacanja.KarticnoPlacanje,
                _ => NacinPlacanja.Gotovina
            };

            try
            {

                var racun = _prodajaServis
                    .Prodaj(izabrano.ParfemId, kolicina, tipProdaje, nacinPlacanja)
                    .GetAwaiter()
                    .GetResult();

                Console.WriteLine("\n==================== FISKALNI RACUN ====================");
                Console.WriteLine($"ID: {racun.Id}");
                Console.WriteLine($"Datum/Vreme: {racun.DatumVreme}");
                Console.WriteLine($"Tip prodaje: {racun.TipProdaje}");
                Console.WriteLine($"Način plaćanja: {racun.NacinPlacanja}");
                Console.WriteLine($"Iznos: {racun.IznosZaNaplatu}");
                Console.WriteLine("Stavke:");
                foreach (var s in racun.Stavke)
                {
                    Console.WriteLine($" - {s.NazivParfema} | kom: {s.Kolicina} | cena: {s.CenaPoKomadu} | ukupno: {s.Ukupno}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška pri prodaji: {ex.Message}");
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
