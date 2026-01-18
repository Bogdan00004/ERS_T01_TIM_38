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
            _proizvodnjaMock = new Mock<IProizvodnjaServis>(MockBehavior.Loose);
            _loggerMock = new Mock<ILoggerServis>(MockBehavior.Loose);

            _sut = new PreradaServis(
                _biljkeRepoMock.Object,
                _parfemiRepoMock.Object,
                _proizvodnjaMock.Object,
                _loggerMock.Object);
        }

        [Test]
        public void PreradiBiljke_NepodrzanaZapremina_VracaPraznuListu_LogujeWarning()
        {
            var parfemi = _sut.PreradiBiljke("Rose", "Parfem", 120m, 1, 100);

            Assert.That(parfemi, Is.Not.Null);
            Assert.That(parfemi.Count, Is.EqualTo(0));
            _loggerMock.Verify(l => l.LogWarning(It.IsAny<string>()), Times.AtLeastOnce);
        }

        [Test]
        public void PreradiBiljke_NulaBocica_VracaPraznuListu_LogujeWarning()
        {
            var parfemi = _sut.PreradiBiljke("Rose", "Parfem", 120m, 0, 150);

            Assert.That(parfemi.Count, Is.EqualTo(0));
            _loggerMock.Verify(l => l.LogWarning(It.IsAny<string>()), Times.AtLeastOnce);
        }

        [Test]
        public void PreradiBiljke_DovoljnoUbranih_150ml_2Bocice_Kreira2Parfema_IzmeniSePoziva12Puta_Dodaj2Puta()
        {
            // 2 * 150 = 300ml => potrebno 6 biljaka
            var ubrane = new List<Biljka>();
            for (int i = 0; i < 6; i++)
                ubrane.Add(new Biljka { Id = Guid.NewGuid(), Naziv = "Rose", Stanje = StanjeBiljke.Ubrana, JacinaAromaticnihUlja = 2.0 });

            _biljkeRepoMock
                .Setup(r => r.VratiPoNazivuIStanji("Rose", StanjeBiljke.Ubrana, 6))
                .Returns(ubrane);

            _biljkeRepoMock.Setup(r => r.Izmeni(It.IsAny<Biljka>())).Returns(true);
            _parfemiRepoMock.Setup(r => r.Dodaj(It.IsAny<Parfem>())).Returns(true);

            var parfemi = _sut.PreradiBiljke("Rose", "Parfem", 120m, 2, 150);

            Assert.That(parfemi.Count, Is.EqualTo(2));
            Assert.That(parfemi.TrueForAll(p => p.Naziv == "Rose"), Is.True);
            Assert.That(parfemi.TrueForAll(p => p.NetoKolicina == 150), Is.True);
            Assert.That(parfemi.TrueForAll(p => p.SerijskiBroj.StartsWith("PP-2025-")), Is.True);

            // Izmeni se poziva:
            // - 6 puta (cuvanje promene jacine)
            // - 6 puta (promena stanja na Preradjena)
            _biljkeRepoMock.Verify(r => r.Izmeni(It.IsAny<Biljka>()), Times.Exactly(12));
            _parfemiRepoMock.Verify(r => r.Dodaj(It.IsAny<Parfem>()), Times.Exactly(2));
        }

        [Test]
        public void PreradiBiljke_AkoIzmeniFailTokomCuvanjaJacine_VracaPraznuListu_LogujeError()
        {
            // 1 * 150ml = 150ml => potrebno 3 biljke
            var ubrane = new List<Biljka>
    {
        new Biljka { Id = Guid.NewGuid(), Naziv="Rose", Stanje=StanjeBiljke.Ubrana, JacinaAromaticnihUlja = 2.0 },
        new Biljka { Id = Guid.NewGuid(), Naziv="Rose", Stanje=StanjeBiljke.Ubrana, JacinaAromaticnihUlja = 2.0 },
        new Biljka { Id = Guid.NewGuid(), Naziv="Rose", Stanje=StanjeBiljke.Ubrana, JacinaAromaticnihUlja = 2.0 }
    };

            _biljkeRepoMock
                .Setup(r => r.VratiPoNazivuIStanji("Rose", StanjeBiljke.Ubrana, 3))
                .Returns(ubrane);

            // Failuje odmah na prvom cuvanju promene jacine (u foreach)
            _biljkeRepoMock
                .Setup(r => r.Izmeni(It.IsAny<Biljka>()))
                .Returns(false);

            // Act
            var parfemi = _sut.PreradiBiljke("Rose", "Parfem", 120m, 1, 150);

            // Assert
            Assert.That(parfemi, Is.Not.Null);
            Assert.That(parfemi.Count, Is.EqualTo(0));

            _loggerMock.Verify(l => l.LogError(It.IsAny<string>()), Times.Once);

            // Ne sme da dodaje parfeme jer je puklo pre kreiranja parfema
            _parfemiRepoMock.Verify(r => r.Dodaj(It.IsAny<Parfem>()), Times.Never);

            // Provera da je Izmeni zaista pozvan (bar jednom)
            _biljkeRepoMock.Verify(r => r.Izmeni(It.IsAny<Biljka>()), Times.Once);
        }

        [Test]
        public void PreradiBiljke_KadNemaDovoljnoUbranih_PozivaProizvodnju_DokRepoNeVratiDovoljno()
        {
            // 1 * 150 = 150ml => potrebno 3 biljke
            var prvaTura = new List<Biljka>
            {
                new Biljka { Id = Guid.NewGuid(), Naziv="Rose", Stanje=StanjeBiljke.Ubrana, JacinaAromaticnihUlja = 2.0 }
            };
            var drugaTura = new List<Biljka>
            {
                new Biljka { Id = Guid.NewGuid(), Naziv="Rose", Stanje=StanjeBiljke.Ubrana, JacinaAromaticnihUlja = 2.0 },
                new Biljka { Id = Guid.NewGuid(), Naziv="Rose", Stanje=StanjeBiljke.Ubrana, JacinaAromaticnihUlja = 2.0 },
                new Biljka { Id = Guid.NewGuid(), Naziv="Rose", Stanje=StanjeBiljke.Ubrana, JacinaAromaticnihUlja = 2.0 }
            };

            _biljkeRepoMock
                .SetupSequence(r => r.VratiPoNazivuIStanji("Rose", StanjeBiljke.Ubrana, 3))
                .Returns(prvaTura)
                .Returns(drugaTura);

            _biljkeRepoMock
                .Setup(r => r.NadjiPrvuPoNazivu("Rose"))
                .Returns(new Biljka { Id = Guid.NewGuid(), LatinskiNaziv = "Lavandula", ZemljaPorekla = "Francuska" });

            _proizvodnjaMock
                .Setup(p => p.PosadiNovuBiljku(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new Biljka { Id = Guid.NewGuid() });

            _proizvodnjaMock
                .Setup(p => p.UberiBiljke("Rose", It.IsAny<int>()))
                .Returns(new List<Biljka> { new Biljka { Id = Guid.NewGuid() } });

            _biljkeRepoMock.Setup(r => r.Izmeni(It.IsAny<Biljka>())).Returns(true);
            _parfemiRepoMock.Setup(r => r.Dodaj(It.IsAny<Parfem>())).Returns(true);

            var parfemi = _sut.PreradiBiljke("Rose", "Parfem", 120m, 1, 150);

            Assert.That(parfemi.Count, Is.EqualTo(1));
            _proizvodnjaMock.Verify(p => p.PosadiNovuBiljku(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.AtLeastOnce);
            _proizvodnjaMock.Verify(p => p.UberiBiljke("Rose", It.IsAny<int>()), Times.AtLeastOnce);
        }
    }
}
