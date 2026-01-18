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
        public bool Izmeni(Ambalaza ambalaza)
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

        public bool Dodaj(Ambalaza ambalaza)
        {
            try
            {
                _baza.Tabele.Ambalaze.Add(ambalaza);
                _baza.SacuvajPromene();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<Ambalaza> VratiSve()
        {
            return _baza.Tabele.Ambalaze;
        }

        public Ambalaza NadjiPoId(Guid id)
        {
            var ambalaza = _baza.Tabele.Ambalaze.FirstOrDefault(a => a.Id == id);
            return ambalaza ?? new Ambalaza(); // nikad null
        }

        public bool Obrisi(Guid id)
        {
            try
            {
                var ambalaza = _baza.Tabele.Ambalaze.FirstOrDefault(a => a.Id == id);
                if (ambalaza == null)
                    return false;
                _baza.Tabele.Ambalaze.Remove(ambalaza);
                _baza.SacuvajPromene(); 
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<Ambalaza> VratiSpakovaneAmbalaze()
        {
            return _baza.Tabele.Ambalaze.Where(a => a.Status == StatusAmbalaze.Spakovana).ToList();
        }
        public bool OznaciKaoPoslate(List<Guid> ambalazeId)
        {
            try
            {
                foreach (var id in ambalazeId)
                {
                    var a = _baza.Tabele.Ambalaze.FirstOrDefault(x => x.Id == id);
                    if (a != null) a.Status = StatusAmbalaze.Poslata;
                }
                _baza.SacuvajPromene();
                return true;
            }
            catch { return false; }
        }
    }
}
