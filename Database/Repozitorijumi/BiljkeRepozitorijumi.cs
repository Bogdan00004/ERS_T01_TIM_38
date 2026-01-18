using Domain.BazaPodataka;
using Domain.Modeli;
using Domain.Repozitorijumi;

namespace Database.Repozitorijumi
{
    public class BiljkeRepozitorijum : IBiljkeRepozitorijum
    {
        private readonly IBazaPodataka _baza;

        public BiljkeRepozitorijum(IBazaPodataka baza)
        {
            _baza = baza;
        }

        public List<Biljka> SveBiljke()
        {
            return _baza.Tabele.Biljke;
        }

        public bool Dodaj(Biljka biljka)
        {
            try
            {
                _baza.Tabele.Biljke.Add(biljka);
                _baza.SacuvajPromene();
                return true;
            }
            catch { return false; }
        }

        public bool Izmeni(Biljka biljka)
        {
            try
            {
                var indeks = _baza.Tabele.Biljke.FindIndex(b => b.Id == biljka.Id);
                if (indeks == -1) return false;

                _baza.Tabele.Biljke[indeks] = biljka;
                _baza.SacuvajPromene();
                return true;
            }
            catch { return false; }
        }

        public bool Obrisi(Guid id)
        {
            try
            {
                int pre = _baza.Tabele.Biljke.Count;
                _baza.Tabele.Biljke.RemoveAll(b => b.Id == id);
                if (_baza.Tabele.Biljke.Count == pre) return false;

                _baza.SacuvajPromene();
                return true;
            }
            catch { return false; }
        }

        public Biljka NadjiPoId(Guid id)
        {
            var b = _baza.Tabele.Biljke.FirstOrDefault(x => x.Id == id);
            return b ?? new Biljka(); // nikad null
        }
        public List<Biljka> VratiPoNazivuIStanji(string naziv, StanjeBiljke stanje, int maxKolicina)
        {
            try
            {
                if (maxKolicina <= 0) return new List<Biljka>();

                return _baza.Tabele.Biljke
                    .Where(b => b.Naziv == naziv && b.Stanje == stanje)
                    .Take(maxKolicina)
                    .ToList();
            }
            catch
            {
                return new List<Biljka>();
            }
        }

        public Biljka NadjiPrvuPoNazivu(string naziv)
        {
            try
            {
                var b = _baza.Tabele.Biljke.FirstOrDefault(x => x.Naziv == naziv);
                return b ?? new Biljka();
            }
            catch
            {
                return new Biljka();
            }
        }
    }
}