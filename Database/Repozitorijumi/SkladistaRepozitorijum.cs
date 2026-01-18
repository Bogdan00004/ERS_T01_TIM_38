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
        public bool UkloniAmbalazuIzSkladista(Guid skladisteId, Guid ambalazaId)
        {
            try
            {
                var s = NadjiPoId(skladisteId);
                if (s.Id == Guid.Empty) return false;

                bool uklonjeno = false;
                for (int i = 0; i < s.AmbalazeId.Count; i++)
                {
                    if (s.AmbalazeId[i] == ambalazaId)
                    {
                        s.AmbalazeId.RemoveAt(i);
                        uklonjeno = true;
                        break;
                    }
                }

                if (!uklonjeno) return false;

                if (s.TrenutniKapacitet > 0) s.TrenutniKapacitet--;

                return Izmeni(s);
            }
            catch { return false; }
        }
    }
}
