using Domain.BazaPodataka;
using Domain.Enumeracije;
using Domain.Modeli;
using Domain.Repozitorijumi;

namespace Database.Repozitorijumi
{
    public class AmbalazeRepozitorijum : IAmbalazaRepozitorijum
    {
        private readonly IBazaPodataka _baza;

        public AmbalazeRepozitorijum(IBazaPodataka baza)
        {
            _baza = baza;
        }

        public void Dodaj(Ambalaza ambalaza)
        {
            _baza.Tabele.Ambalaze.Add(ambalaza);
        }

        public List<Ambalaza> VratiSve()
        {
            return _baza.Tabele.Ambalaze;
        }

        public Ambalaza? NadjiPoId(Guid id)
        {
            return _baza.Tabele.Ambalaze.FirstOrDefault(a => a.Id == id);
        }

        public void Obrisi(Guid id)
        {
            var ambalaza = NadjiPoId(id);
            if (ambalaza != null)
                _baza.Tabele.Ambalaze.Remove(ambalaza);
        }

        public void SacuvajPromene()
        {
            _baza.SacuvajPromene();
        }

        public List<Ambalaza> VratiSpakovaneAmbalaze()
        {
            return _baza.Tabele.Ambalaze.Where(a => a.Status == StatusAmbalaze.Spakovana).ToList();
        }
    }
}
