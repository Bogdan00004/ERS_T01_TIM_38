
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
        private readonly ILoggerServis _logger;



        public PakovanjeServis(
        IParfemRepozitorijum parfemiRepozitorijum,
        IAmbalazaRepozitorijum ambalazaRepozitorijum,
        ISkladistaRepozitorijum skladistaRepozitorijum,
        IPreradaServis preradaServis,
        ILoggerServis logger)
        {
            _parfemiRepozitorijum = parfemiRepozitorijum;
            _ambalazaRepozitorijum = ambalazaRepozitorijum;
            _skladistaRepozitorijum = skladistaRepozitorijum;
            _preradaServis = preradaServis;
            _logger = logger;
        }

        public void SpakujParfeme(string nazivParfema, int brojBocica, int zapreminaPoBocici, Guid skladisteId)
        {
            _logger.LogInfo($"Pakovanje: naziv={nazivParfema}, brojBocica={brojBocica}, zapremina={zapreminaPoBocici}, skladisteId={skladisteId}");
            var parfemi = _preradaServis.PreradiBiljke(nazivParfema, brojBocica, zapreminaPoBocici);
            _logger.LogInfo($"Pakovanje: dobijeno parfema={parfemi.Count}");

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


            var ambalaza = dostupneAmbalaze.FirstOrDefault(a => a.Status == StatusAmbalaze.Spakovana && a.SkladisteId==skladisteId);

            if (ambalaza == null)
                throw new Exception("Nema dostupnih spakovanih ambalaza.");


            ambalaza.Status = StatusAmbalaze.Poslata;
            skladiste.TrenutniKapacitet++;
            skladiste.AmbalazeId.Add(ambalaza.Id); 


            _ambalazaRepozitorijum.SacuvajPromene();
            _skladistaRepozitorijum.SacuvajPromene();
            _logger.LogInfo($"Pakovanje završeno: ambalazaId={ambalaza.Id}, parfemaUAmbalazi={ambalaza.ParfemiId.Count}");

        }
    }
}
