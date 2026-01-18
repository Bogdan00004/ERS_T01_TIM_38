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
        public bool Dodaj(Parfem parfem)
        {
            try
            {
                _baza.Tabele.Parfemi.Add(parfem);
                _baza.SacuvajPromene();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public List<Parfem> SviParfemi()
        {
            return _baza.Tabele.Parfemi.ToList();
        }
        public Parfem NadjiPoId(Guid id)
        {
            var parfem = _baza.Tabele.Parfemi.FirstOrDefault(p => p.Id == id);
            return parfem ?? new Parfem(); // nikad null
        }
        public bool Izmeni(Parfem parfem)
        {
            try
            {
                var indeks = _baza.Tabele.Parfemi.FindIndex(p => p.Id == parfem.Id);
                if (indeks == -1)
                    return false;

                _baza.Tabele.Parfemi[indeks] = parfem;
                _baza.SacuvajPromene();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Obrisi(Guid id)
        {
            try
            {
                var p = _baza.Tabele.Parfemi.FirstOrDefault(x => x.Id == id);
                if (p == null) return false;
                _baza.Tabele.Parfemi.Remove(p);
                _baza.SacuvajPromene();
                return true;
            }
            catch { return false; }
        }
    }
}
