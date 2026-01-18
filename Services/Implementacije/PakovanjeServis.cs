
using Domain.Enumeracije;
using Domain.Modeli;
using Domain.Repozitorijumi;
using Domain.Servisi;

namespace Loger_Bloger.Servisi
{
    public class PakovanjeServis : IPakovanjeServis
    {
        private readonly IParfemRepozitorijum _parfemiRepozitorijum;
        private readonly IAmbalazaRepozitorijum _ambalazaRepozitorijum;
        private readonly ISkladistaRepozitorijum _skladistaRepozitorijum;
        private readonly IPreradaServis _preradaServis;
        private readonly ILoggerServis _logger;



        public PakovanjeServis(IParfemRepozitorijum parfemiRepozitorijum, IAmbalazaRepozitorijum ambalazaRepozitorijum, ISkladistaRepozitorijum skladistaRepozitorijum, IPreradaServis preradaServis, ILoggerServis logger)
        {
            _parfemiRepozitorijum = parfemiRepozitorijum;
            _ambalazaRepozitorijum = ambalazaRepozitorijum;
            _skladistaRepozitorijum = skladistaRepozitorijum;
            _preradaServis = preradaServis;
            _logger = logger;
        }

        public bool SpakujParfeme(string nazivParfema, string tip, decimal cena, int brojBocica, int zapreminaPoBocici, Guid skladisteId)
        {
            _logger.LogInfo($"[Pakovanje] SpakujParfeme: naziv={nazivParfema}, brojBocica={brojBocica}, zapremina={zapreminaPoBocici}, skladisteId={skladisteId}");

            if (brojBocica <= 0)
            {
                _logger.LogWarning("[Pakovanje] Neispravan broj bocica (<=0).");
                return false;
            }

            var skladiste = _skladistaRepozitorijum.NadjiPoId(skladisteId);
            if (skladiste.Id == Guid.Empty)
            {
                _logger.LogError("[Pakovanje] Skladiste ne postoji.");
                return false;
            }

            // PreradaServis po pravilima treba da vrati praznu listu ako ne moze
            var parfemi = _preradaServis.PreradiBiljke(nazivParfema, tip, cena, brojBocica, zapreminaPoBocici);
            if (parfemi == null || parfemi.Count == 0)
            {
                _logger.LogWarning("[Pakovanje] Prerada nije vratila parfeme - pakovanje se prekida.");
                return false;
            }

            var ambalaza = new Ambalaza(
                naziv: $"Ambalaza-{DateTime.Now:yyyyMMdd-HHmmss}",
                adresaPosiljaoca: "O'Sinjel De Or, Paris",
                skladisteId: skladisteId,
                status: StatusAmbalaze.Spakovana);

            // Bez LINQ
            for (int i = 0; i < parfemi.Count; i++)
            {
                ambalaza.ParfemiId.Add(parfemi[i].Id);
            }

            if (!_ambalazaRepozitorijum.Dodaj(ambalaza))
            {
                _logger.LogError("[Pakovanje] Neuspesno dodavanje ambalaze u repozitorijum.");
                return false;
            }

            _logger.LogInfo($"[Pakovanje] Ambalaza spakovana: ambalazaId={ambalaza.Id}, parfemaUAmbalazi={ambalaza.ParfemiId.Count}");
            return true;
        }

        public bool PosaljiAmbalazuUSkladiste(Guid skladisteId)
        {
            var skladiste = _skladistaRepozitorijum.NadjiPoId(skladisteId);
            if (skladiste.Id == Guid.Empty)
            {
                _logger.LogError("[Pakovanje] Skladiste ne postoji.");
                return false;
            }

            if (skladiste.TrenutniKapacitet >= skladiste.MaxKapacitet)
            {
                _logger.LogWarning("[Pakovanje] Skladiste je popunjeno.");
                return false;
            }

            var dostupneAmbalaze = _ambalazaRepozitorijum.VratiSpakovaneAmbalaze();
            if (dostupneAmbalaze == null || dostupneAmbalaze.Count == 0)
            {
                _logger.LogWarning("[Pakovanje] Nema spakovanih ambalaza.");
                return false;
            }

            Ambalaza pronadjena = new Ambalaza();
            for (int i = 0; i < dostupneAmbalaze.Count; i++)
            {
                var a = dostupneAmbalaze[i];
                if (a.SkladisteId != skladisteId) continue;

                bool vecUSkladistu = false;
                for (int j = 0; j < skladiste.AmbalazeId.Count; j++)
                {
                    if (skladiste.AmbalazeId[j] == a.Id)
                    {
                        vecUSkladistu = true;
                        break;
                    }
                }
                if (!vecUSkladistu)
                {
                    pronadjena = a;
                    break;
                }
            }

            if (pronadjena.Id == Guid.Empty)
            {
                _logger.LogWarning("[Pakovanje] Nema dostupne spakovane ambalaze za ovo skladiste.");
                return false;
            }

            skladiste.TrenutniKapacitet++;
            skladiste.AmbalazeId.Add(pronadjena.Id);

            if (!_skladistaRepozitorijum.Izmeni(skladiste))
            {
                _logger.LogError("[Pakovanje] Neuspesno cuvanje promena skladista.");
                return false;
            }

            _logger.LogInfo($"[Pakovanje] Ambalaza poslata u skladiste: ambalazaId={pronadjena.Id}, kapacitet={skladiste.TrenutniKapacitet}/{skladiste.MaxKapacitet}");
            return true;
        }

        public bool ObezbediAmbalazuUSkladistu(string nazivParfema, string tip, decimal cena, int brojBocica, int zapreminaPoBocici, Guid skladisteId)
        {
            _logger.LogInfo($"[Pakovanje] ObezbediAmbalazuUSkladistu: naziv={nazivParfema}, brojBocica={brojBocica}, zapremina={zapreminaPoBocici}, skladisteId={skladisteId}");

            var skladiste = _skladistaRepozitorijum.NadjiPoId(skladisteId);
            if (skladiste.Id == Guid.Empty)
            {
                _logger.LogError("[Pakovanje] Skladiste ne postoji.");
                return false;
            }

            if (skladiste.TrenutniKapacitet >= skladiste.MaxKapacitet)
            {
                _logger.LogWarning("[Pakovanje] Skladiste je popunjeno. Nema slobodnog kapaciteta.");
                return false;
            }

            // 1) pokusaj da “ubacis” postojecu spakovanu
            bool uspeh = PosaljiAmbalazuUSkladiste(skladisteId);
            if (uspeh) return true;

            // 2) ako nema, pokreni pakovanje (koje pravi novu spakovanu ambalazu)
            _logger.LogWarning("[Pakovanje] Nema dostupne ambalaze - pokrecem pakovanje.");
            if (!SpakujParfeme(nazivParfema, tip, cena, brojBocica, zapreminaPoBocici, skladisteId))
            {
                _logger.LogError("[Pakovanje] Pakovanje nije uspelo, ne mogu da obezbedim ambalazu.");
                return false;
            }

            // 3) posalji novu u skladiste
            uspeh = PosaljiAmbalazuUSkladiste(skladisteId);
            if (!uspeh)
            {
                _logger.LogError("[Pakovanje] Napravio sam ambalazu, ali nisam uspeo da je ubacim u skladiste.");
                return false;
            }
            return true;
        }
    }
}
