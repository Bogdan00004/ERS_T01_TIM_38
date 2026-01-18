using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Modeli;
using Domain.Repozitorijumi;
using Domain.BazaPodataka;

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

        public void Dodaj(Biljka biljka)
        {
            _baza.Tabele.Biljke.Add(biljka);
            _baza.SacuvajPromene();
        }

        public void Izmeni(Biljka biljka)
        {
            var indeks = _baza.Tabele.Biljke.FindIndex(b => b.Id == biljka.Id);
            if (indeks != -1)
            {
                _baza.Tabele.Biljke[indeks] = biljka;
                _baza.SacuvajPromene();
            }
        }

        public void Obrisi(Guid id)
        {
            _baza.Tabele.Biljke.RemoveAll(b => b.Id == id);
            _baza.SacuvajPromene();
        }

        public Biljka? NadjiPoId(Guid id)
        {
            return _baza.Tabele.Biljke.FirstOrDefault(b => b.Id == id);
        }
    }
}