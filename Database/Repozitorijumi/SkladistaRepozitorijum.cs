using Domain.BazaPodataka;
using Domain.Modeli;
using Domain.Repozitorijumi;

namespace Database.Repozitorijumi
{
    public class SkladistaRepozitorijum : ISkladistaRepozitorijum
    {
        private readonly IBazaPodataka _baza;

        public SkladistaRepozitorijum(IBazaPodataka baza)
        {
            _baza = baza;
        }

        public bool Dodaj(Skladiste skladiste)
        {
            try
            {
                _baza.Tabele.Skladista.Add(skladiste);
                _baza.SacuvajPromene();  
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Skladiste NadjiPoId(Guid id)
        {
            var s = _baza.Tabele.Skladista.FirstOrDefault(x => x.Id == id);
            return s ?? new Skladiste(); // nikad null
        }

        public List<Skladiste> VratiSva()
        {
            return _baza.Tabele.Skladista;
        }
        public bool Izmeni(Skladiste skladiste)
        {
            try
            {
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
                var s = _baza.Tabele.Skladista.FirstOrDefault(x => x.Id == id);
                if (s == null) return false;

                _baza.Tabele.Skladista.Remove(s);
                _baza.SacuvajPromene();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
