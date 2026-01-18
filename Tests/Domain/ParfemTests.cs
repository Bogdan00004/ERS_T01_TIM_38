using Domain.Modeli;
using NUnit.Framework;
using System.Text.Json;

namespace Tests.Domain
{
    [TestFixture]
    public class ParfemTests
    {
        [Test]
        public void Json_RoundTrip_Cuva_Sva_Polja()
        {
            var id = Guid.NewGuid();

            var p = new Parfem
            {
                Id = id,
                Naziv = "Noir Rose",
                Tip = "Parfem",
                NetoKolicina = 150,
                SerijskiBroj = $"PP-2025-{id}",
                Cena = 99.99m,
                BiljkaId = Guid.NewGuid(),
                RokTrajanja = new DateTime(2027, 1, 1, 12, 0, 0, DateTimeKind.Utc)
            };

            string json = JsonSerializer.Serialize(p);
            var p2 = JsonSerializer.Deserialize<Parfem>(json);

            Assert.That(p2, Is.Not.Null);

            Assert.That(p2!.Id, Is.EqualTo(p.Id));
            Assert.That(p2.Naziv, Is.EqualTo(p.Naziv));
            Assert.That(p2.Tip, Is.EqualTo(p.Tip));
            Assert.That(p2.NetoKolicina, Is.EqualTo(p.NetoKolicina));
            Assert.That(p2.SerijskiBroj, Is.EqualTo(p.SerijskiBroj));
            Assert.That(p2.Cena, Is.EqualTo(p.Cena));
            Assert.That(p2.BiljkaId, Is.EqualTo(p.BiljkaId));
            Assert.That(p2.RokTrajanja, Is.EqualTo(p.RokTrajanja));
        }
    }
}
