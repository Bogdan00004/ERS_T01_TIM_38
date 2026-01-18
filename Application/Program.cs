using Database.BazaPodataka;
using Database.Repozitorijumi;
using Domain.BazaPodataka;
using Domain.Enumeracije;
using Domain.Repozitorijumi;
using Domain.Servisi;
using Loger_Bloger.Servisi.Skladistenje;
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
            IBiljkeRepozitorijum biljkeRepozitorijum = new BiljkeRepozitorijum(bazaPodataka);


            // parfemi + racuni
            IParfemRepozitorijum parfemRepozitorijum = new ParfemiRepozitorijum(bazaPodataka);
            IFiskalniRacunRepozitorijum fiskalniRacunRepozitorijum = new FiskalniRacuniRepozitorijum(bazaPodataka);
            ILoggerServis logger = new TekstualniLoggerServis("log.txt");

            // Servisi
            IProizvodnjaServis proizvodnjaServis = new Services.Implementacije.ProizvodnjaServis(biljkeRepozitorijum, logger);
            IPreradaServis preradaServis = new Services.Implementacije.PreradaServis(biljkeRepozitorijum, parfemRepozitorijum, proizvodnjaServis, logger);
            IPakovanjeServis pakovanjeServis = new Loger_Bloger.Servisi.PakovanjeServis(parfemRepozitorijum, ambalazaRepozitorijum, skladistaRepozitorijum, preradaServis, logger);
            IAutentifikacijaServis autentifikacijaServis = new AutentifikacioniServis(korisniciRepozitorijum, logger);

            var magacinskiServis = new MagacinskiCentarServis(skladistaRepozitorijum, ambalazaRepozitorijum, logger);
            var distribucioniServis = new DistribucioniCentarServis(skladistaRepozitorijum, ambalazaRepozitorijum, logger);
            ISkladistenjeServisUloge uloga = new SkladistenjeServisUloge(magacinskiServis, distribucioniServis);



            // Login
            Func<TipKorisnika, IProdajaServis> prodajaServisFactory = (tipKorisnika) =>
            {
                ISkladistenjeServis skladistenjeServis = uloga.KreirajServis(tipKorisnika);

                return new Loger_Bloger.Servisi.Prodaja.ProdajaServis(
                    parfemRepozitorijum,
                    ambalazaRepozitorijum,
                    fiskalniRacunRepozitorijum,
                    skladistenjeServis,
                    pakovanjeServis,
                    skladistaRepozitorijum,
                    logger);
            };

            // Koristi autentifikacioni meni iz Presentation/Autentifikacija
            var autentifikacioniMeni = new Presentation.Authentifikacija.AutentifikacioniMeni(
                autentifikacijaServis,
                uloga,
                prodajaServisFactory
            );

            autentifikacioniMeni.Pokreni();

        }
    }
}
