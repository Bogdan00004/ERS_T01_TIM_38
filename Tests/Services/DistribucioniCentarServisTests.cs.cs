using Domain.Enumeracije;
using Domain.Modeli;
using Domain.Repozitorijumi;
using Domain.Servisi;
using Loger_Bloger.Servisi.Skladistenje;
using Moq;
using NUnit.Framework;

namespace Tests.Services
{
    [TestFixture]
    public class DistribucioniCentarServisTests
    {
        private Mock<ISkladistaRepozitorijum> _skladistaRepoMock = null!;
        private Mock<IAmbalazaRepozitorijum> _ambalazeRepoMock = null!;
        private Mock<ILoggerServis> _loggerMock = null!;
        private DistribucioniCentarServis _sut = null!;

        [SetUp]
        public void Setup()
        {
            _skladistaRepoMock = new Mock<ISkladistaRepozitorijum>(MockBehavior.Loose);
            _ambalazeRepoMock = new Mock<IAmbalazaRepozitorijum>(MockBehavior.Loose);
            _loggerMock = new Mock<ILoggerServis>(MockBehavior.Loose);

            _sut = new DistribucioniCentarServis(
                _skladistaRepoMock.Object,
                _ambalazeRepoMock.Object,
                _loggerMock.Object
            );
        }

        [Test]
        public async Task PosaljiAmbalazeProdaji_KadImaSpakovanih_Vrati1_OznaciPoslata_IzmeniSkladiste_IRepoOznaci()
        {
            // Arrange
            var skladiste1 = new Skladiste("S1", "L1", 10);
            var skladista = new List<Skladiste> { skladiste1 };

            var amb1 = new Ambalaza("A1", "Adresa", skladiste1.Id, StatusAmbalaze.Spakovana);

            _skladistaRepoMock
                .Setup(r => r.VratiSva())
                .Returns(skladista);

            // Repo već vraća spakovane koje su u skladištu (bez LINQ u servisu)
            _ambalazeRepoMock
                .Setup(r => r.VratiSpakovaneKojeSuUSkladistu(skladista, 1))
                .Returns(new List<Ambalaza> { amb1 });

            // Repo uklanja ambalazu iz skladista
            _skladistaRepoMock
                .Setup(r => r.UkloniAmbalazuIzSkladista(amb1.SkladisteId, amb1.Id))
                .Returns(true);

            // Repo oznacava poslate
            _ambalazeRepoMock
                .Setup(r => r.OznaciKaoPoslate(It.IsAny<List<Guid>>()))
                .Returns(true);

            // Act
            var poslate = await _sut.PosaljiAmbalazeProdaji(1);

            // Assert
            Assert.That(poslate, Is.Not.Null);
            Assert.That(poslate.Count, Is.EqualTo(1));
            Assert.That(poslate[0].Id, Is.EqualTo(amb1.Id));
            Assert.That(poslate[0].Status, Is.EqualTo(StatusAmbalaze.Poslata));

            _skladistaRepoMock.Verify(r => r.VratiSva(), Times.Once);
            _ambalazeRepoMock.Verify(r => r.VratiSpakovaneKojeSuUSkladistu(skladista, 1), Times.Once);
            _skladistaRepoMock.Verify(r => r.UkloniAmbalazuIzSkladista(amb1.SkladisteId, amb1.Id), Times.Once);

            _ambalazeRepoMock.Verify(r =>
                r.OznaciKaoPoslate(It.Is<List<Guid>>(ids => ids.Count == 1 && ids[0] == amb1.Id)),
                Times.Once);

            _loggerMock.Verify(l => l.LogInfo(It.IsAny<string>()), Times.AtLeastOnce);
        }
    }
}
