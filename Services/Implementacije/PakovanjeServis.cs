
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

        public void SpakujParfeme(string nazivParfema, string tip, decimal cena, int brojBocica, int zapreminaPoBocici, Guid skladisteId)
        {
            _logger.LogInfo($"Pakovanje: naziv={nazivParfema}, brojBocica={brojBocica}, zapremina={zapreminaPoBocici}, skladisteId={skladisteId}");
            var parfemi = _preradaServis.PreradiBiljke(nazivParfema, tip, cena, brojBocica, zapreminaPoBocici);

            if (brojBocica <= 0)
                throw new ArgumentException("Broj bocica mora biti > 0.");

            var skladiste = _skladistaRepozitorijum.NadjiPoId(skladisteId);
            if (skladiste == null)
                throw new Exception("Skladiste ne postoji.");


            var ambalaza = new Ambalaza
            {
                Naziv = $"Ambalaza-{DateTime.Now:yyyyMMdd-HHmmss}",
                AdresaPosiljaoca = "O'Sinjel De Or, Paris",
                SkladisteId = skladisteId,
                Status = StatusAmbalaze.Spakovana
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


            var ambalaza = dostupneAmbalaze.FirstOrDefault(a => a.SkladisteId == skladisteId && !skladiste.AmbalazeId.Contains(a.Id));

            if (ambalaza == null)
                throw new Exception("Nema dostupnih spakovanih ambalaza.");



            skladiste.TrenutniKapacitet++;
            skladiste.AmbalazeId.Add(ambalaza.Id);



            _skladistaRepozitorijum.SacuvajPromene();
            _logger.LogInfo($"Pakovanje završeno: ambalazaId={ambalaza.Id}, parfemaUAmbalazi={ambalaza.ParfemiId.Count}");

        }
        public void ObezbediAmbalazuUSkladistu(string nazivParfema, string tip, decimal cena, int brojBocica, int zapreminaPoBocici, Guid skladisteId)
        {
            _logger.LogInfo($"[Pakovanje] ObezbediAmbalazuUSkladistu: naziv={nazivParfema}, brojBocica={brojBocica}, zapremina={zapreminaPoBocici}, skladisteId={skladisteId}");

            var skladiste = _skladistaRepozitorijum.NadjiPoId(skladisteId);
            if (skladiste == null)
                throw new Exception("Skladiste ne postoji.");

            if (skladiste.TrenutniKapacitet >= skladiste.MaxKapacitet)
                throw new Exception("Skladiste je popunjeno. Nema slobodnog kapaciteta za novu ambalazu.");

            // 1) Probaj da ubacis postojecu spakovanu ambalazu za to skladiste
            var dostupneAmbalaze = _ambalazaRepozitorijum.VratiSpakovaneAmbalaze();
            var ambalaza = dostupneAmbalaze
                .FirstOrDefault(a => a.SkladisteId == skladisteId && !skladiste.AmbalazeId.Contains(a.Id));

            // 2) Ako nema dostupne zapocni pakovanje
            if (ambalaza == null)
            {
                _logger.LogWarning("[Pakovanje] Nema dostupne spakovane ambalaze - pokrecem pakovanje.");

                SpakujParfeme(nazivParfema, tip, cena, brojBocica, zapreminaPoBocici, skladisteId);

                // ponovo ucitaj spakovane i uzmi novu
                dostupneAmbalaze = _ambalazaRepozitorijum.VratiSpakovaneAmbalaze();
                ambalaza = dostupneAmbalaze
                    .FirstOrDefault(a => a.SkladisteId == skladisteId && !skladiste.AmbalazeId.Contains(a.Id));

                if (ambalaza == null)
                    throw new Exception("Nije moguće obezbediti ambalazu (pakovanje neuspešno).");
            }

            // 3) Ubaci u skladiste 
            skladiste.TrenutniKapacitet++;
            skladiste.AmbalazeId.Add(ambalaza.Id);

            _skladistaRepozitorijum.SacuvajPromene();

            _logger.LogInfo($"[Pakovanje] Ambalaza obezbedjena u skladistu: ambalazaId={ambalaza.Id}, kapacitet={skladiste.TrenutniKapacitet}/{skladiste.MaxKapacitet}");
        }

    }
}
