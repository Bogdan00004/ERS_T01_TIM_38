using Domain.BazaPodataka;
using Domain.Modeli;
using Domain.Repozitorijumi;

namespace Database.Repozitorijumi
{
    public class ParfemiRepozitorijum : IParfemRepozitorijum
    {
        private readonly IBazaPodataka _baza;

        public ParfemiRepozitorijum(IBazaPodataka baza)
        {
            _baza = baza;
        }
        public void Dodaj(Parfem parfem)
        {
            _baza.Tabele.Parfemi.Add(parfem);
            _baza.SacuvajPromene();
        }
        public List<Parfem> SviParfemi()
        {
            return _baza.Tabele.Parfemi.ToList();
        }
        public Parfem? NadjiPoId(Guid id)
        {
            return _baza.Tabele.Parfemi.FirstOrDefault(p => p.Id == id);
        }
    }
}
