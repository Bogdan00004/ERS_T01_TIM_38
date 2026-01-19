using Domain.Enumeracije;
using Domain.Modeli;
using Domain.PomocneMetode.Prodaja;
using Domain.Repozitorijumi;
using Domain.Servisi;

namespace Loger_Bloger.Servisi.Prodaja
{
    public class ProdajaServis : IProdajaServis
    {
        private readonly IParfemRepozitorijum _parfemiRepo;
        private readonly IAmbalazaRepozitorijum _ambalazeRepo;
        private readonly IFiskalniRacunRepozitorijum _racuniRepo;
        private readonly ISkladistenjeServis _skladistenjeServis;
        private readonly IPakovanjeServis _pakovanjeServis;
        private readonly ISkladistaRepozitorijum _skladistaRepo;
        private readonly ILoggerServis _logger;
        private readonly IKatalogRepozitorijum _katalogRepo;

        public ProdajaServis(IParfemRepozitorijum parfemiRepo, IAmbalazaRepozitorijum ambalazeRepo, IFiskalniRacunRepozitorijum racuniRepo, ISkladistenjeServis skladistenjeServis, IPakovanjeServis pakovanjeServis, ISkladistaRepozitorijum skladistaRepo, ILoggerServis logger, IKatalogRepozitorijum katalogRepo)
        {
            _parfemiRepo = parfemiRepo;
            _ambalazeRepo = ambalazeRepo;
            _racuniRepo = racuniRepo;
            _skladistenjeServis = skladistenjeServis;
            _pakovanjeServis = pakovanjeServis;
            _skladistaRepo = skladistaRepo;
            _logger = logger;
            _katalogRepo = katalogRepo;
        }

        private FiskalniRacun Fail(string poruka, bool error = true)
        {
            if (error) _logger.LogError(poruka);
            else _logger.LogWarning(poruka);
            return new FiskalniRacun();
        }
        private int RaspolozivoZa(Parfem izabrani)
        {
            return _katalogRepo.VratiRaspolozivo(izabrani.Naziv, izabrani.Tip, izabrani.NetoKolicina);
        }

        private async Task<List<Ambalaza>> PreuzmiAmbalaze(int kolicinaBocica, Dictionary<Guid, Parfem> mapaParfema, string trazeniKey, Guid parfemId)
        {
            var preuzete = new List<Ambalaza>();
            int skupljeno = 0;
            int pokusaji = 0;

            while (skupljeno < kolicinaBocica)
            {
                pokusaji++;
                if (pokusaji > ProdajaPomocneMetode.MAX_POKUSAJA_PREUZIMANJA)
                {
                    _logger.LogError($"[Prodaja] Previše pokušaja preuzimanja ambalaža (parfemId={parfemId}).");
                    return new List<Ambalaza>();
                }

                int josTreba = kolicinaBocica - skupljeno;
                int trazeneAmbalaze = Math.Min(josTreba, ProdajaPomocneMetode.MAX_AMBALAZA_PO_TURI);

                var nove = await _skladistenjeServis.PosaljiAmbalazeProdaji(trazeneAmbalaze);
                if (nove == null || nove.Count == 0)
                {
                    _logger.LogWarning($"[Prodaja] Skladište nema dostupnih spakovanih ambalaža (parfemId={parfemId}).");

                    return new List<Ambalaza>();
                }

                for (int i = 0; i < nove.Count; i++)
                    preuzete.Add(nove[i]);

                skupljeno = ProdajaPomocneMetode.PrebrojSkupljeno(preuzete, mapaParfema, trazeniKey);
            }

            return preuzete;
        }

        public async Task<FiskalniRacun> Prodaj(Guid parfemId, int kolicinaBocica, TipProdaje tipProdaje, NacinPlacanja nacinPlacanja)
        {
            _logger.LogInfo($"[Prodaja] Početak prodaje: parfemId={parfemId}, kolicina={kolicinaBocica}, tip={tipProdaje}, nacin={nacinPlacanja}");

            if (kolicinaBocica <= 0)
                return Fail($"[Prodaja] Neispravna količina: {kolicinaBocica}", error: false);

            var izabrani = _parfemiRepo.NadjiPoId(parfemId);
            if (izabrani.Id == Guid.Empty)
                return Fail($"[Prodaja] Parfem ne postoji: parfemId={parfemId}");

            // Priprema mape parfema i ključa
            var mapaParfema = ProdajaPomocneMetode.NapraviMapuParfema(_parfemiRepo);
            string trazeniKey = ProdajaKljucHelper.NapraviKljuc(izabrani);

            //PRE ORDER
            int raspolozivo = RaspolozivoZa(izabrani);

            int izLagera = Math.Min(raspolozivo, kolicinaBocica);
            int preOrder = kolicinaBocica - izLagera;

            if (preOrder > 0)
            {
                _logger.LogWarning($"[Prodaja] PRE-ORDER: trazeno={kolicinaBocica}, raspolozivo={raspolozivo}, " + $"isporuceno={izLagera}, preorder={preOrder}");
                _logger.LogInfo($"[Prodaja] PRE-ORDER se naplaćuje odmah za celu količinu: {kolicinaBocica}.");

            }
            // Ako imamo nešto na lageru – skidamo taj deo
            if (izLagera > 0)
            {
                var preuzete = await PreuzmiAmbalaze(izLagera, mapaParfema, trazeniKey, parfemId);
                if (preuzete == null || preuzete.Count == 0)
                    return Fail("[Prodaja] Neuspešno preuzimanje ambalaža (delimična isporuka).");

                if (!ProdajaPomocneMetode.SkiniParfeme(preuzete, izLagera, mapaParfema, trazeniKey, _logger))
                    return Fail("[Prodaja] Neuspešno skidanje parfema iz ambalaža.");

                if (!ProdajaPomocneMetode.SacuvajAmbalaze(_ambalazeRepo, preuzete, "[Prodaja] Neuspešno čuvanje promena ambalaža.", _logger))
                    return Fail("[Prodaja] Neuspešno čuvanje ambalaža.");

                if (!ProdajaPomocneMetode.ObradiStatusIVracanje(_skladistaRepo, _ambalazeRepo, preuzete, _logger))
                    return Fail("[Prodaja] Neuspešno vraćanje ambalaža u skladište.");
            }

            // Ako postoji ijedan pre-order komad -> račun je PreOrder
            TipProdaje finalniTip = (preOrder > 0) ? TipProdaje.PreOrder : tipProdaje;

            var racun = ProdajaPomocneMetode.KreirajRacun(izabrani, kolicinaBocica, finalniTip, nacinPlacanja);

            if (!_racuniRepo.Dodaj(racun))
                return Fail("[Prodaja] Neuspešno čuvanje fiskalnog računa.");

            _logger.LogInfo($"[Prodaja] Završena prodaja: parfem={izabrani.Naziv}, " + $"trazeno={kolicinaBocica}, isporuceno={izLagera}, preorder={preOrder}, tip={finalniTip}");

            return racun;
        }
        public List<FiskalniRacun> VratiSveRacune()
        {
            return _racuniRepo.VratiSve();
        }
        public List<KatalogStavka> VratiKatalogDostupnihParfema()
        {
            return _katalogRepo.VratiKatalogDostupnihParfema();
        }
    }
}
