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

            Console.WriteLine("\n================================== KATALOG PARFEMA ==================================");
            if (katalog == null || katalog.Count == 0)
            {
                Console.WriteLine("Nema parfema u sistemu.");
                return;
            }

            // Header tabele
            Console.WriteLine("--------------------------------------------------------------------------------------");
            Console.WriteLine(string.Format("{0,-4} {1,-22} {2,-12} {3,6} {4,10} {5,12}",
                "RB", "NAZIV", "TIP", "ML", "CENA", "RASPOLOZ."));
            Console.WriteLine("--------------------------------------------------------------------------------------");

            for (int i = 0; i < katalog.Count; i++)
            {
                var k = katalog[i];

                string status = (k.Raspolozivo > 0) ? "NA STANJU" : "PRE-ORDER";

                Console.WriteLine(string.Format("{0,-4} {1,-22} {2,-12} {3,6} {4,10:0.##} {5,12}   {6}",
                    i + 1,
                    Skrati(k.Naziv, 22),
                    Skrati(k.Tip, 12),
                    k.NetoKolicina,
                    k.Cena,
                    k.Raspolozivo,
                    status));
            }

            Console.WriteLine("--------------------------------------------------------------------------------------");
            Console.WriteLine("NAPOMENA: Ako je RASPOLOZ. = 0, prodaja ide kao PRE-ORDER (isporuka kada stigne na lager).");

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

            if (izabrano.Raspolozivo == 0)
            {
                Console.WriteLine("\nIzabrani parfem trenutno NIJE na stanju.");
                Console.Write("Prodaja će biti PRE-ORDER (isporuka naknadno). Nastaviti? (D/N): ");
                var potvrda = (Console.ReadLine() ?? "").Trim().ToUpperInvariant();

                if (potvrda != "D")
                {
                    Console.WriteLine("Prekid prodaje.");
                    return;
                }
            }

            // =================== TIP PRODAJE (VALIDACIJA) ===================
            Console.WriteLine("\nTip prodaje:");
            Console.WriteLine("1) Maloprodaja");
            Console.WriteLine("2) Veleprodaja");
            Console.Write("Izbor: ");

            var tp = (Console.ReadLine() ?? "").Trim();
            TipProdaje tipProdaje;

            if (tp == "1") tipProdaje = TipProdaje.Maloprodaja;
            else if (tp == "2") tipProdaje = TipProdaje.Veleprodaja;
            else
            {
                Console.WriteLine("Neispravan izbor tipa prodaje.");
                return;
            }

            // =================== NACIN PLACANJA (VALIDACIJA) ===================
            Console.WriteLine("\nNačin plaćanja:");
            Console.WriteLine("1) Gotovina");
            Console.WriteLine("2) Uplata na račun");
            Console.WriteLine("3) Kartično plaćanje");
            Console.Write("Izbor: ");

            var np = (Console.ReadLine() ?? "").Trim();
            NacinPlacanja nacinPlacanja;

            if (np == "1") nacinPlacanja = NacinPlacanja.Gotovina;
            else if (np == "2") nacinPlacanja = NacinPlacanja.UplataNaRacun;
            else if (np == "3") nacinPlacanja = NacinPlacanja.KarticnoPlacanje;
            else
            {
                Console.WriteLine("Neispravan izbor načina plaćanja.");
                return;
            }

            var racun = _prodajaServis
                .Prodaj(izabrano.ParfemId, kolicina, tipProdaje, nacinPlacanja)
                .GetAwaiter()
                .GetResult();

            if (racun == null || racun.Id == Guid.Empty)
            {
                Console.WriteLine("Prodaja nije uspešna (nije moguće obraditi zahtev).");
                return;
            }

            Console.WriteLine("\n================================== FISKALNI RACUN ==================================");
            Console.WriteLine($"ID:          {racun.Id}");
            Console.WriteLine($"Datum/Vreme: {racun.DatumVreme}");
            Console.WriteLine($"Tip prodaje: {racun.TipProdaje}");
            Console.WriteLine($"Plaćanje:    {racun.NacinPlacanja}");
            Console.WriteLine($"Iznos:       {racun.IznosZaNaplatu:0.##}");
            Console.WriteLine("--------------------------------------------------------------------------------------");
            Console.WriteLine("STAVKE:");
            for (int i = 0; i < racun.Stavke.Count; i++)
            {
                var s = racun.Stavke[i];
                Console.WriteLine($" - {s.NazivParfema} | kom: {s.Kolicina} | cena: {s.CenaPoKomadu:0.##} | ukupno: {s.Ukupno:0.##}");
            }
            Console.WriteLine("--------------------------------------------------------------------------------------");
        }
        private void PrikaziSveRacune()
        {
            var racuni = _prodajaServis.VratiSveRacune();

            Console.WriteLine("\n================================== SVI FISKALNI RACUNI ==================================");

            if (racuni == null || racuni.Count == 0)
            {
                Console.WriteLine("Nema fiskalnih računa.");
                return;
            }

            // Header
            Console.WriteLine("------------------------------------------------------------------------------------------");
            Console.WriteLine(string.Format("{0,-3} {1,-12} {2,-19} {3,-12} {4,-16} {5,10}",
                "RB", "ID", "DATUM/VREME", "TIP", "PLACANJE", "IZNOS"));
            Console.WriteLine("------------------------------------------------------------------------------------------");

            for (int i = 0; i < racuni.Count; i++)
            {
                var r = racuni[i];

                string idShort = (r.Id == Guid.Empty) ? "N/A" : r.Id.ToString().Substring(0, 8);
                string dt = r.DatumVreme.ToString("dd.MM.yyyy HH:mm");
                string tip = r.TipProdaje.ToString();
                string placanje = r.NacinPlacanja.ToString();
                decimal iznos = r.IznosZaNaplatu;

                Console.WriteLine(string.Format("{0,-3} {1,-12} {2,-19} {3,-12} {4,-16} {5,10:0.##}",
                    i + 1,
                    idShort,
                    dt,
                    Skrati(tip, 12),
                    Skrati(placanje, 16),
                    iznos));
            }

            Console.WriteLine("------------------------------------------------------------------------------------------");
            Console.WriteLine("Unesite RB računa za detalje (stavke) ili 0 za povratak.");
            Console.Write("RB: ");

            if (!int.TryParse(Console.ReadLine(), out int rb) || rb < 0 || rb > racuni.Count)
            {
                Console.WriteLine("Neispravan unos.");
                return;
            }

            if (rb == 0) return;

            var racun = racuni[rb - 1];

            Console.WriteLine("\n================================== DETALJI RACUNA ==================================");
            Console.WriteLine($"ID:          {racun.Id}");
            Console.WriteLine($"Datum/Vreme: {racun.DatumVreme}");
            Console.WriteLine($"Tip prodaje: {racun.TipProdaje}");
            Console.WriteLine($"Plaćanje:    {racun.NacinPlacanja}");
            Console.WriteLine($"Iznos:       {racun.IznosZaNaplatu:0.##}");
            Console.WriteLine("--------------------------------------------------------------------------------------");
            Console.WriteLine(string.Format("{0,-3} {1,-26} {2,6} {3,10} {4,12}",
                "RB", "NAZIV", "KOM", "CENA", "UKUPNO"));
            Console.WriteLine("--------------------------------------------------------------------------------------");

            for (int i = 0; i < racun.Stavke.Count; i++)
            {
                var s = racun.Stavke[i];
                Console.WriteLine(string.Format("{0,-3} {1,-26} {2,6} {3,10:0.##} {4,12:0.##}",
                    i + 1,
                    Skrati(s.NazivParfema, 26),
                    s.Kolicina,
                    s.CenaPoKomadu,
                    s.Ukupno));
            }

            Console.WriteLine("--------------------------------------------------------------------------------------");
        }
        private static string Skrati(string? s, int max)
        {
            if (string.IsNullOrWhiteSpace(s)) return "";
            s = s.Trim();
            if (s.Length <= max) return s;
            return s.Substring(0, max - 3) + "...";
        }
    }
}
