using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Modeli;
using Domain.Enumeracije;
using Domain.Servisi;

namespace Presentation.Meni
{
    public class AutentifikacioniMeni
    {
        private readonly IAutentifikacijaServis _autentifikacijaServis;
        private readonly ISkladistenjeServisUloge _ulogeServis;
        private readonly Func<TipKorisnika, IProdajaServis> _prodajaServisFactory;

        public AutentifikacioniMeni(IAutentifikacijaServis autentifikacijaServis, ISkladistenjeServisUloge ulogeServis,Func<TipKorisnika, IProdajaServis> prodajaServisFactory)
        {
            _autentifikacijaServis = autentifikacijaServis;
            _ulogeServis = ulogeServis;
            _prodajaServisFactory = prodajaServisFactory;
        }


        public void Pokreni()
        {
            Korisnik ulogovaniKorisnik = null;

            while (ulogovaniKorisnik == null)
            {
                Console.WriteLine("Dobrodošli! Izaberite opciju:");
                Console.WriteLine("1. Prijava");
                Console.WriteLine("2. Registracija");
                Console.Write("Opcija: ");
                var opcija = Console.ReadLine();

                if (opcija == "1")
                {
                    Console.Write("Korisničko ime: ");
                    var korisnickoIme = Console.ReadLine();
                    Console.Write("Lozinka: ");
                    var lozinka = Console.ReadLine();

                    var (uspesno, korisnik) = _autentifikacijaServis.Prijava(korisnickoIme, lozinka);

                    if (uspesno)
                    {
                        ulogovaniKorisnik = korisnik;
                        Console.WriteLine("Uspešna prijava!");
                    }
                    else
                    {
                        Console.WriteLine("Neuspešna prijava. Pokušajte ponovo.");
                    }
                }
                else if (opcija == "2")
                {
                    Console.Write("Ime i prezime: ");
                    var imePrezime = Console.ReadLine();
                    Console.Write("Korisničko ime: ");
                    var korisnickoIme = Console.ReadLine();
                    Console.Write("Lozinka: ");
                    var lozinka = Console.ReadLine();

                    Console.WriteLine("Izaberite tip korisnika:");
                    Console.WriteLine("1. Menadzer prodaje");
                    Console.WriteLine("2. Prodavac");
                    Console.Write("Opcija: ");
                    var izborTipa = Console.ReadLine();

                    TipKorisnika tipKorisnika = izborTipa == "1" ? TipKorisnika.MenadzerProdaje : TipKorisnika.Prodavac;

                    var noviKorisnik = new Korisnik(korisnickoIme, lozinka, imePrezime, tipKorisnika);
                    var (uspesno, korisnik) = _autentifikacijaServis.Registracija(noviKorisnik);

                    if (uspesno)
                    {
                        ulogovaniKorisnik = korisnik;
                        Console.WriteLine("Uspešna registracija!");
                    }
                    else
                    {
                        Console.WriteLine("Korisnik sa tim korisničkim imenom već postoji.");
                    }
                }
                else
                {
                    Console.WriteLine("Nepoznata opcija.");
                }
            }

            var prodajaServis = _prodajaServisFactory(ulogovaniKorisnik.Uloga);

            var meni = new OpcijeMeni(ulogovaniKorisnik, prodajaServis);
            meni.PrikaziMeni();

        }
    }
}
