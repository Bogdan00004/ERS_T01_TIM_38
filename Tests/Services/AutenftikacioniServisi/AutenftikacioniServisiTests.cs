using NUnit.Framework;
using Domain.Enumeracije;
using Domain.Modeli;
using Domain.Repozitorijumi;
using Domain.Servisi;
using Moq;
using Services.AutentifikacioniServisi;

namespace Tests.Services.AutenftikacioniServisi
{
    [TestFixture]
    public class AutentifikacioniServisTests
    {
        private Mock<IKorisniciRepozitorijum> _korisniciRepoMock = null!;
        private Mock<ILoggerServis> _loggerMock = null!;
        private AutentifikacioniServis _sut = null!;

        [SetUp]
        public void Setup()
        {
            _korisniciRepoMock = new Mock<IKorisniciRepozitorijum>(MockBehavior.Strict);
            _loggerMock = new Mock<ILoggerServis>(MockBehavior.Loose);

            _sut = new AutentifikacioniServis(_korisniciRepoMock.Object, _loggerMock.Object);
        }

        [Test]
        public void Prijava_TacnaLozinka_VracaTrueIVracaKorisnika_LogujeInfo()
        {
            
            var k = new Korisnik("admin", "pass", "Admin Admin", TipKorisnika.MenadzerProdaje);
            _korisniciRepoMock
                .Setup(r => r.PronadjiKorisnikaPoKorisnickomImenu("admin"))
                .Returns(k);

           
            var (ok, korisnik) = _sut.Prijava("admin", "pass");

            
            Assert.That(ok, Is.True);
            Assert.That(korisnik, Is.SameAs(k));

            _loggerMock.Verify(l => l.LogInfo(It.Is<string>(s => s.Contains("Prijava uspešna"))), Times.Once);
            _loggerMock.Verify(l => l.LogWarning(It.IsAny<string>()), Times.Never);

            _korisniciRepoMock.VerifyAll();
        }

        [Test]
        public void Prijava_PogresnaLozinka_VracaFalse_LogujeWarning()
        {
            
            var k = new Korisnik("admin", "pass", "Admin Admin", TipKorisnika.MenadzerProdaje);
            _korisniciRepoMock
                .Setup(r => r.PronadjiKorisnikaPoKorisnickomImenu("admin"))
                .Returns(k);

            
            var (ok, korisnik) = _sut.Prijava("admin", "WRONG");

            
            Assert.That(ok, Is.False);
            Assert.That(korisnik, Is.Not.Null);

            _loggerMock.Verify(l => l.LogWarning(It.Is<string>(s => s.Contains("Neuspešna prijava"))), Times.Once);

            _korisniciRepoMock.VerifyAll();
        }

        [Test]
        public void Registracija_NoviKorisnik_DodajeKorisnika_LogujeUspeh()
        {
            
            var novi = new Korisnik("novo", "123", "Novi Novi", TipKorisnika.Prodavac);

            _korisniciRepoMock
                .Setup(r => r.PronadjiKorisnikaPoKorisnickomImenu("novo"))
                .Returns((Korisnik)null!);

            _korisniciRepoMock
                .Setup(r => r.DodajKorisnika(It.IsAny<Korisnik>()))
                .Returns(novi);

            
            var (ok, _) = _sut.Registracija(novi);

            
            Assert.That(ok, Is.True);
            _korisniciRepoMock.Verify(r => r.DodajKorisnika(It.Is<Korisnik>(x => x.KorisnickoIme == "novo")), Times.Once);

            _loggerMock.Verify(l => l.LogInfo(It.Is<string>(s => s.Contains("Pokušaj registracije"))), Times.Once);
            _loggerMock.Verify(l => l.LogInfo(It.Is<string>(s => s.Contains("Registracija uspešna"))), Times.Once);
            _loggerMock.Verify(l => l.LogWarning(It.IsAny<string>()), Times.Never);

            _korisniciRepoMock.VerifyAll();
        }

        [Test]
        public void Registracija_ZauzetoKorisnickoIme_NeDodajeKorisnika_LogujeWarning()
        {
            
            var novi = new Korisnik("admin", "123", "Novi Novi", TipKorisnika.Prodavac);
            var postojeci = new Korisnik("admin", "pass", "Admin Admin", TipKorisnika.MenadzerProdaje);

            _korisniciRepoMock
                .Setup(r => r.PronadjiKorisnikaPoKorisnickomImenu("admin"))
                .Returns(postojeci);

            
            var (ok, _) = _sut.Registracija(novi);

            
            Assert.That(ok, Is.False);

            _korisniciRepoMock.Verify(r => r.DodajKorisnika(It.IsAny<Korisnik>()), Times.Never);
            _loggerMock.Verify(l => l.LogWarning(It.Is<string>(s => s.Contains("Registracija odbijena"))), Times.Once);

            _korisniciRepoMock.VerifyAll();
        }
    }
}