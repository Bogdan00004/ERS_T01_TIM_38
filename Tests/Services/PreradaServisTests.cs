using Domain.Modeli;
using Domain.Repozitorijumi;
using Domain.Servisi;
using Moq;
using NUnit.Framework;
using Services.Implementacije;



namespace Tests.Services
{
    [TestFixture]
    public class PreradaServisTests
    {
        private Mock<IBiljkeRepozitorijum> _biljkeRepoMock = null!;
        private Mock<IParfemRepozitorijum> _parfemiRepoMock = null!;
        private Mock<IProizvodnjaServis> _proizvodnjaMock = null!;
        private Mock<ILoggerServis> _loggerMock = null!;
        private PreradaServis _sut = null!;

        [SetUp]
        public void Setup()
        {
            _biljkeRepoMock = new Mock<IBiljkeRepozitorijum>(MockBehavior.Loose);
            _parfemiRepoMock = new Mock<IParfemRepozitorijum>(MockBehavior.Loose);
            _loggerMock = new Mock<ILoggerServis>(MockBehavior.Loose);

            _proizvodnjaMock = new Mock<IProizvodnjaServis>(MockBehavior.Loose);
            _sut = new PreradaServis(_biljkeRepoMock.Object, _parfemiRepoMock.Object, _proizvodnjaMock.Object, _loggerMock.Object);

        }

        [Test]
        public void PreradiBiljke_NepodrzanaZapremina_BacaArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _sut.PreradiBiljke("Rose", "Parfem", 120m, 1, 100));
        }

        [Test]
        public void PreradiBiljke_KadNemaDovoljnoUbranihBiljaka_PozivaProizvodnjuI_NeBaca()
        {

            var biljke = new List<Biljka>
             {
                new Biljka { Id = Guid.NewGuid(), Naziv="Rose", Stanje = StanjeBiljke.Ubrana }
                  };


            _biljkeRepoMock.Setup(r => r.SveBiljke()).Returns(() => biljke);

            _proizvodnjaMock
                .Setup(p => p.PosadiNovuBiljku(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback(() =>
                {
                    biljke.Add(new Biljka { Id = Guid.NewGuid(), Naziv = "Rose", Stanje = StanjeBiljke.Posadjena });
                    biljke.Add(new Biljka { Id = Guid.NewGuid(), Naziv = "Rose", Stanje = StanjeBiljke.Posadjena });
                })
                .Returns(() => new Biljka { Id = Guid.NewGuid(), Naziv = "Rose", Stanje = StanjeBiljke.Posadjena });

            _proizvodnjaMock
                .Setup(p => p.UberiBiljke("Rose", It.IsAny<int>()))
                .Callback(() =>
                {

                    foreach (var b in biljke.Where(x => x.Naziv == "Rose" && x.Stanje == StanjeBiljke.Posadjena))
                        b.Stanje = StanjeBiljke.Ubrana;
                })
                .Returns(() => biljke.Where(x => x.Naziv == "Rose" && x.Stanje == StanjeBiljke.Ubrana).ToList());

            Assert.DoesNotThrow(() => _sut.PreradiBiljke("Rose", "Parfem", 120m, 1, 150));
            _proizvodnjaMock.Verify(p => p.PosadiNovuBiljku(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.AtLeastOnce);
            _proizvodnjaMock.Verify(p => p.UberiBiljke("Rose", It.IsAny<int>()), Times.AtLeastOnce);
        }


        [Test]
        public void PreradiBiljke_DovoljnoUbranihBiljaka_KreiraParfeme_IzmeniStanjeBiljaka()
        {

            var biljke = Enumerable.Range(0, 6)
                .Select(_ => new Biljka { Id = Guid.NewGuid(), Naziv = "Rose", Stanje = StanjeBiljke.Ubrana })
                .ToList();

            _biljkeRepoMock.Setup(r => r.SveBiljke()).Returns(biljke);

            var izmenjeneBiljke = new List<Biljka>();
            _biljkeRepoMock
                .Setup(r => r.Izmeni(It.IsAny<Biljka>()))
                .Callback<Biljka>(b => izmenjeneBiljke.Add(b));

            var dodatiParfemi = new List<Parfem>();
            _parfemiRepoMock
                .Setup(r => r.Dodaj(It.IsAny<Parfem>()))
                .Callback<Parfem>(p => dodatiParfemi.Add(p));


            var parfemi = _sut.PreradiBiljke("Rose", "Parfem", 120m, 2, 150);


            Assert.That(parfemi.Count, Is.EqualTo(2));
            Assert.That(parfemi.All(p => p.Naziv == "Rose"), Is.True);
            Assert.That(parfemi.All(p => p.NetoKolicina == 150), Is.True);
            Assert.That(parfemi.All(p => p.SerijskiBroj.StartsWith("PP-2025-")), Is.True);

            _parfemiRepoMock.Verify(r => r.Dodaj(It.IsAny<Parfem>()), Times.Exactly(2));
            Assert.That(dodatiParfemi.Count, Is.EqualTo(2));


            _biljkeRepoMock.Verify(r => r.Izmeni(It.IsAny<Biljka>()), Times.Exactly(6));
            Assert.That(izmenjeneBiljke.Count, Is.EqualTo(6));
            Assert.That(izmenjeneBiljke.All(b => b.Stanje == StanjeBiljke.Preradjena), Is.True);
        }
    }
}
