using Domain.BazaPodataka;
using Domain.Modeli;
using Domain.Servisi;
using Moq;
using NUnit.Framework;
using Services.Implementacije;

namespace Tests.Services
{
    [TestFixture]
    public class ProizvodnjaServisTests
    {
        private Mock<IBazaPodataka> _bazaMock = null!;
        private Mock<ILoggerServis> _loggerMock = null!;
        private TabeleBazaPodataka _tabele = null!;
        private ProizvodnjaServis _sut = null!;

        [SetUp]
        public void Setup()
        {
            _tabele = new TabeleBazaPodataka
            {
                Biljke = new List<Biljka>()
            };

            _bazaMock = new Mock<IBazaPodataka>(MockBehavior.Loose);
            _bazaMock.SetupProperty(b => b.Tabele, _tabele);
            _bazaMock.Setup(b => b.SacuvajPromene()).Returns(true);

            _loggerMock = new Mock<ILoggerServis>(MockBehavior.Loose);

            _sut = new ProizvodnjaServis(_bazaMock.Object, _loggerMock.Object);
        }

        [Test]
        public void PosadiNovuBiljku_DodajeBiljkuIStavljaStanjePosadjena_IzazivaSacuvajPromene()
        {

            var nova = _sut.PosadiNovuBiljku("Lavanda", "Lavandula", "Francuska");

            Assert.That(nova, Is.Not.Null);
            Assert.That(_tabele.Biljke.Count, Is.EqualTo(1));
            Assert.That(_tabele.Biljke[0].Id, Is.EqualTo(nova.Id));
            Assert.That(_tabele.Biljke[0].Stanje, Is.EqualTo(StanjeBiljke.Posadjena));

        }

        [Test]
        public void PromeniJacinuUlja_AkoJeJacinaPreradjeneBiljkeIznad4_SmanjujeNaProcenatOdstupanja_65postoZa465()
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
            _tabele.Biljke.Add(biljka);


            _sut.PromeniJacinuUlja(biljka.Id, 65);


            Assert.That(biljka.JacinaAromaticnihUlja, Is.EqualTo(1.30).Within(0.0001));
            _bazaMock.Verify(b => b.SacuvajPromene(), Times.Once);
        }

        [Test]
        public void PromeniJacinuUlja_AkoJeJacinaPreradjeneBiljkeManjaIliJednaka4_NeMenjaJacinu_LogujeInfo()
        {

            var biljka = new Biljka
            {
                Id = Guid.NewGuid(),
                Naziv = "Lavanda",
                JacinaAromaticnihUlja = 3.33,
                Stanje = StanjeBiljke.Posadjena,
                LatinskiNaziv = "Lavandula",
                ZemljaPorekla = "Francuska"
            };
            _tabele.Biljke.Add(biljka);


            _sut.PromeniJacinuUlja(biljka.Id, 100);


            Assert.That(biljka.JacinaAromaticnihUlja, Is.EqualTo(3.33).Within(0.0001));
            _loggerMock.Verify(l => l.LogInfo(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void PromeniJacinuUlja_AkoBiljkaNePostoji_LogujeError_IzlaziBezSacuvajPromene()
        {

            _sut.PromeniJacinuUlja(Guid.NewGuid(), 4.65);


            _loggerMock.Verify(l => l.LogError(It.IsAny<string>()), Times.Once);
            _bazaMock.Verify(b => b.SacuvajPromene(), Times.Never);
        }

        [Test]
        public void UberiBiljke_MenjaStanjePosadjenihNaUbrana_VracaTrazeniBroj()
        {

            _tabele.Biljke.Add(new Biljka { Naziv = "Lavanda", Stanje = StanjeBiljke.Posadjena });
            _tabele.Biljke.Add(new Biljka { Naziv = "Lavanda", Stanje = StanjeBiljke.Posadjena });
            _tabele.Biljke.Add(new Biljka { Naziv = "Lavanda", Stanje = StanjeBiljke.Posadjena });


            var ubrane = _sut.UberiBiljke("Lavanda", 2);


            Assert.That(ubrane.Count, Is.EqualTo(2));
            Assert.That(ubrane.TrueForAll(b => b.Stanje == StanjeBiljke.Ubrana), Is.True);
            _bazaMock.Verify(b => b.SacuvajPromene(), Times.Once);
        }
    }
}
