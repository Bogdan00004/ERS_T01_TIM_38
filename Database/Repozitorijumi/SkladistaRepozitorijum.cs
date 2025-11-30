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
    public class SkladistaRepozitorijum:ISkladistaRepozitorijum
    {
        private readonly IBazaPodataka _baza;

        public SkladistaRepozitorijum(IBazaPodataka baza)
        {
            _baza = baza;
        }

        public void Dodaj(Skladiste skladiste)
        {
            _baza.Tabele.Skladista.Add(skladiste);
        }

        public Skladiste? NadjiPoId(Guid id)
        {
            return _baza.Tabele.Skladista.FirstOrDefault(s => s.Id == id);
        }

        public List<Skladiste> VratiSva()
        {
            return _baza.Tabele.Skladista;
        }

        public void SacuvajPromene()
        {
            _baza.SacuvajPromene();
        }
    }
}
