using Domain.Enumeracije;
using Domain.Modeli;
using NUnit.Framework;
using System.Text.Json;

namespace Tests.Domain
{
    [TestFixture]
    public class AmbalazaTests
    {
        [Test]
        public void ParametarskiKonstruktor_Generise_Id_I_Listu()
        {
            var a = new Ambalaza("Kutija-1", "Paris, Rue 1", Guid.NewGuid(), StatusAmbalaze.Spakovana);

            Assert.That(a.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(a.ParfemiId, Is.Not.Null);
            Assert.That(a.Status, Is.EqualTo(StatusAmbalaze.Spakovana));
        }

        [Test]
        public void Json_RoundTrip_Cuva_Sva_Polja()
        {
            var a = new Ambalaza
            {
                Id = Guid.NewGuid(),
                Naziv = "Kutija-1",
                AdresaPosiljaoca = "Paris, Rue 1",
                SkladisteId = Guid.NewGuid(),
                ParfemiId = new System.Collections.Generic.List<Guid> { Guid.NewGuid(), Guid.NewGuid() },
                Status = StatusAmbalaze.Poslata
            };

            string json = JsonSerializer.Serialize(a);
            var a2 = JsonSerializer.Deserialize<Ambalaza>(json);

            Assert.That(a2, Is.Not.Null);

            Assert.That(a2!.Id, Is.EqualTo(a.Id));
            Assert.That(a2.Naziv, Is.EqualTo(a.Naziv));
            Assert.That(a2.AdresaPosiljaoca, Is.EqualTo(a.AdresaPosiljaoca));
            Assert.That(a2.SkladisteId, Is.EqualTo(a.SkladisteId));
            Assert.That(a2.Status, Is.EqualTo(a.Status));

            Assert.That(a2.ParfemiId, Is.Not.Null);
            Assert.That(a2.ParfemiId.Count, Is.EqualTo(a.ParfemiId.Count));
            Assert.That(a2.ParfemiId, Is.EquivalentTo(a.ParfemiId));
        }
    }
}
