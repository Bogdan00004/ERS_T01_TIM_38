using Database.Repozitorijumi;
using Domain.Enumeracije;
using Domain.Modeli;
using Domain.Repozitorijumi;
using Domain.Servisi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loger_Bloger.Servisi
{
    public class PakovanjeServis : IPakovanjeServis
    {
        private readonly IParfemRepozitorijum _parfemiRepozitorijum;
        private readonly IAmbalazaRepozitorijum _ambalazaRepozitorijum;
        private readonly ISkladistaRepozitorijum _skladistaRepozitorijum;
        private readonly IPreradaServis _preradaServis;


        public PakovanjeServis(
        IParfemRepozitorijum parfemiRepozitorijum,
        IAmbalazaRepozitorijum ambalazaRepozitorijum,
        ISkladistaRepozitorijum skladistaRepozitorijum,
        IPreradaServis preradaServis)
        {
            _parfemiRepozitorijum = parfemiRepozitorijum;
            _ambalazaRepozitorijum = ambalazaRepozitorijum;
            _skladistaRepozitorijum = skladistaRepozitorijum;
            _preradaServis = preradaServis;
        }

        public void SpakujParfeme(string nazivParfema, int brojBocica, double zapreminaPoBocici, Guid skladisteId)
        {
            var parfemi = _preradaServis.PreradiBiljke(nazivParfema, brojBocica, zapreminaPoBocici);

            foreach (var parfem in parfemi)
            {
                _parfemiRepozitorijum.Dodaj(parfem);
            }

            var ambalaza = new Ambalaza
            {
                SkladisteId = skladisteId
            };

            foreach (var parfem in parfemi)
            {
                ambalaza.ParfemiId.Add(parfem.Id);
            }

            _ambalazaRepozitorijum.Dodaj(ambalaza);
            _ambalazaRepozitorijum.SacuvajPromene();
        }

        public void PosaljiAmbalazuUSkladiste(Guid skladisteId)
        {
            var skladiste = _skladistaRepozitorijum.NadjiPoId(skladisteId);
            var dostupneAmbalaze = _ambalazaRepozitorijum.VratiSpakovaneAmbalaze();


            if (skladiste == null)
                throw new Exception("Skladiste ne postoji.");


            if (skladiste.TrenutniKapacitet >= skladiste.MaxKapacitet)
                throw new Exception("Skladiste je popunjeno.");


            var ambalaza = dostupneAmbalaze.FirstOrDefault(a => a.Status == StatusAmbalaze.Spakovana);

            if (ambalaza == null)
                throw new Exception("Nema dostupnih spakovanih ambalaza.");


            ambalaza.Status = StatusAmbalaze.Poslata;
            skladiste.TrenutniKapacitet++;


            _ambalazaRepozitorijum.SacuvajPromene();
            _skladistaRepozitorijum.SacuvajPromene();
        }
    }
}
