using Domain.Modeli;
using Domain.Repozitorijumi;
using Domain.Servisi;
using Moq;
using NUnit.Framework;
using Services.Implementacije;

namespace Tests.Services
{
    [TestFixture]
    public class ProizvodnjaServisTests
    {
        private Mock<IBiljkeRepozitorijum> _biljkeRepoMock = null!;
        private Mock<ILoggerServis> _loggerMock = null!;
        private ProizvodnjaServis _sut = null!;

        [SetUp]
        public void Setup()
        {
            _biljkeRepoMock = new Mock<IBiljkeRepozitorijum>(MockBehavior.Loose);
            _loggerMock = new Mock<ILoggerServis>(MockBehavior.Loose);

            _sut = new ProizvodnjaServis(_biljkeRepoMock.Object, _loggerMock.Object);
        }

        [Test]
        public void PosadiNovuBiljku_DodajeBiljku_StanjePosadjena()
        {
            _biljkeRepoMock
                .Setup(r => r.Dodaj(It.IsAny<Biljka>()))
                .Returns(true);

            var nova = _sut.PosadiNovuBiljku("Lavanda", "Lavandula", "Francuska");

            Assert.That(nova.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(nova.Naziv, Is.EqualTo("Lavanda"));
            Assert.That(nova.Stanje, Is.EqualTo(StanjeBiljke.Posadjena));

            _biljkeRepoMock.Verify(r => r.Dodaj(It.Is<Biljka>(b =>
                b.Naziv == "Lavanda" &&
                b.LatinskiNaziv == "Lavandula" &&
                b.ZemljaPorekla == "Francuska" &&
                b.Stanje == StanjeBiljke.Posadjena
            )), Times.Once);
        }

        [Test]
        public void PosadiNovuBiljku_AkoDodajFail_VracaPraznuBiljku_LogujeError()
        {
            _biljkeRepoMock
                .Setup(r => r.Dodaj(It.IsAny<Biljka>()))
                .Returns(false);

            var nova = _sut.PosadiNovuBiljku("Lavanda", "Lavandula", "Francuska");

            Assert.That(nova.Id, Is.EqualTo(Guid.Empty));
            _loggerMock.Verify(l => l.LogError(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void PromeniJacinuUlja_65postoOd200Je130_IzmeniSePoziva()
        {
            var biljka = new Biljka
            {
                Id = Guid.NewGuid(),
                Naziv = "Lavanda",
                JacinaAromaticnihUlja = 2.00,
                Stanje = StanjeBiljke.Posadjena,
                LatinskiNaziv = "Lavandula",
                ZemljaPorekla = "Francuska"
            };

            _biljkeRepoMock.Setup(r => r.NadjiPoId(biljka.Id)).Returns(biljka);
            _biljkeRepoMock.Setup(r => r.Izmeni(It.IsAny<Biljka>())).Returns(true);

            var ok = _sut.PromeniJacinuUlja(biljka.Id, 65);

            Assert.That(ok, Is.True);
            Assert.That(biljka.JacinaAromaticnihUlja, Is.EqualTo(1.30).Within(0.0001));
            _biljkeRepoMock.Verify(r => r.Izmeni(It.IsAny<Biljka>()), Times.Once);
        }

        [Test]
        public void PromeniJacinuUlja_AkoBiljkaNePostoji_VracaFalse_LogujeError_IzmeniSeNePoziva()
        {
            _biljkeRepoMock.Setup(r => r.NadjiPoId(It.IsAny<Guid>())).Returns(new Biljka());

            var ok = _sut.PromeniJacinuUlja(Guid.NewGuid(), 65);

            Assert.That(ok, Is.False);
            _loggerMock.Verify(l => l.LogError(It.IsAny<string>()), Times.Once);
            _biljkeRepoMock.Verify(r => r.Izmeni(It.IsAny<Biljka>()), Times.Never);
        }

        [Test]
        public void PromeniJacinuUlja_AkoIzmeniFail_VracaFalse_LogujeError()
        {
            var biljka = new Biljka
            {
                Id = Guid.NewGuid(),
                Naziv = "Lavanda",
                JacinaAromaticnihUlja = 2.00,
                Stanje = StanjeBiljke.Posadjena
            };

            _biljkeRepoMock.Setup(r => r.NadjiPoId(biljka.Id)).Returns(biljka);
            _biljkeRepoMock.Setup(r => r.Izmeni(It.IsAny<Biljka>())).Returns(false);

            var ok = _sut.PromeniJacinuUlja(biljka.Id, 65);

            Assert.That(ok, Is.False);
            _loggerMock.Verify(l => l.LogError(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void UberiBiljke_VracaPraznoAkoKolicinaNijePozitivna()
        {
            var ubrane = _sut.UberiBiljke("Lavanda", 0);

            Assert.That(ubrane, Is.Not.Null);
            Assert.That(ubrane.Count, Is.EqualTo(0));
            _biljkeRepoMock.Verify(r => r.VratiPoNazivuIStanji(It.IsAny<string>(), It.IsAny<StanjeBiljke>(), It.IsAny<int>()), Times.Never);
        }

        [Test]
        public void UberiBiljke_MenjaStanjeNaUbrana_VracaTrazeniBroj_IzmeniSePozivaZaSvaku()
        {
            var b1 = new Biljka { Id = Guid.NewGuid(), Naziv = "Lavanda", Stanje = StanjeBiljke.Posadjena };
            var b2 = new Biljka { Id = Guid.NewGuid(), Naziv = "Lavanda", Stanje = StanjeBiljke.Posadjena };

            _biljkeRepoMock.Setup(r => r.VratiPoNazivuIStanji("Lavanda", StanjeBiljke.Posadjena, 2)).Returns(new List<Biljka> { b1, b2 });

            _biljkeRepoMock.Setup(r => r.Izmeni(It.IsAny<Biljka>())).Returns(true);

            var ubrane = _sut.UberiBiljke("Lavanda", 2);

            Assert.That(ubrane.Count, Is.EqualTo(2));
            Assert.That(ubrane.TrueForAll(x => x.Stanje == StanjeBiljke.Ubrana), Is.True);

            _biljkeRepoMock.Verify(r => r.Izmeni(It.IsAny<Biljka>()), Times.Exactly(2));
        }

        [Test]
        public void UberiBiljke_AkoIzmeniJedneBiljkeFail_VracaPraznuListu_LogujeError()
        {
            var b1 = new Biljka { Id = Guid.NewGuid(), Naziv = "Lavanda", Stanje = StanjeBiljke.Posadjena };
            var b2 = new Biljka { Id = Guid.NewGuid(), Naziv = "Lavanda", Stanje = StanjeBiljke.Posadjena };

            _biljkeRepoMock
                .Setup(r => r.VratiPoNazivuIStanji("Lavanda", StanjeBiljke.Posadjena, 2))
                .Returns(new List<Biljka> { b1, b2 });

            _biljkeRepoMock.SetupSequence(r => r.Izmeni(It.IsAny<Biljka>())).Returns(true).Returns(false);

            var ubrane = _sut.UberiBiljke("Lavanda", 2);

            Assert.That(ubrane.Count, Is.EqualTo(0));
            _loggerMock.Verify(l => l.LogError(It.IsAny<string>()), Times.Once);
        }
    }
}
