using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Modeli;
using Domain.Repozitorijumi;
using Moq;
using NUnit.Framework;
using Services.Implementacije;
using Domain.Servisi;



namespace Tests.Services
{
    [TestFixture]
    public class PreradaServisTests
    {
        private Mock<IBiljkeRepozitorijum> _biljkeRepoMock = null!;
        private Mock<IParfemRepozitorijum> _parfemiRepoMock = null!;
        private Mock<ILoggerServis> _loggerMock = null!;
        private PreradaServis _sut = null!;

        [SetUp]
        public void Setup()
        {
            _biljkeRepoMock = new Mock<IBiljkeRepozitorijum>(MockBehavior.Loose);
            _parfemiRepoMock = new Mock<IParfemRepozitorijum>(MockBehavior.Loose);
            _loggerMock = new Mock<ILoggerServis>(MockBehavior.Loose);


            _sut = new PreradaServis(_biljkeRepoMock.Object, _parfemiRepoMock.Object, _loggerMock.Object);

        }

        [Test]
        public void PreradiBiljke_NepodrzanaZapremina_BacaArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _sut.PreradiBiljke("Rose", 1, 100));
        }

        [Test]
        public void PreradiBiljke_NemaDovoljnoUbranihBiljaka_BacaInvalidOperationException()
        {
            
            var biljke = new List<Biljka>
            {
                new Biljka { Id = Guid.NewGuid(), Stanje = StanjeBiljke.Ubrana },
                new Biljka { Id = Guid.NewGuid(), Stanje = StanjeBiljke.Ubrana },
            };

            _biljkeRepoMock.Setup(r => r.SveBiljke()).Returns(biljke);

            Assert.Throws<InvalidOperationException>(() => _sut.PreradiBiljke("Rose", 1, 150));
        }

        [Test]
        public void PreradiBiljke_DovoljnoUbranihBiljaka_KreiraParfeme_IzmeniStanjeBiljaka()
        {
           
            var biljke = Enumerable.Range(0, 6)
                .Select(_ => new Biljka { Id = Guid.NewGuid(), Stanje = StanjeBiljke.Ubrana })
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

            
            var parfemi = _sut.PreradiBiljke("Rose", 2, 150);

            
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
