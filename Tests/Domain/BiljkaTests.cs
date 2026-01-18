using Domain.Modeli;
using NUnit.Framework;
using System.Text.Json;

namespace Tests.Domain
{
    [TestFixture]
    public class BiljkaTests
    {
        [Test]
        public void Konstruktor_Generise_Id_Ima_Default_Stanje()
        {
            var b = new Biljka();

            Assert.That(b.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(b.Stanje, Is.EqualTo(StanjeBiljke.Posadjena));
        }

        [Test]
        public void Json_RoundTrip_Cuva_Sva_Polja()
        {
            var b = new Biljka
            {
                Id = Guid.NewGuid(),
                Naziv = "Lavanda",
                JacinaAromaticnihUlja = 4.25, // u specifikaciji opseg 1.0–5.0
                LatinskiNaziv = "Lavandula",
                ZemljaPorekla = "Francuska",
                Stanje = StanjeBiljke.Ubrana
            };

            string json = JsonSerializer.Serialize(b);
            var b2 = JsonSerializer.Deserialize<Biljka>(json);

            Assert.That(b2, Is.Not.Null);

            Assert.That(b2!.Id, Is.EqualTo(b.Id));
            Assert.That(b2.Naziv, Is.EqualTo(b.Naziv));
            Assert.That(b2.JacinaAromaticnihUlja, Is.EqualTo(b.JacinaAromaticnihUlja).Within(0.0001));
            Assert.That(b2.LatinskiNaziv, Is.EqualTo(b.LatinskiNaziv));
            Assert.That(b2.ZemljaPorekla, Is.EqualTo(b.ZemljaPorekla));
            Assert.That(b2.Stanje, Is.EqualTo(b.Stanje));
        }
    }
}
