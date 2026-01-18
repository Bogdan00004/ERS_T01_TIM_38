using Domain.Enumeracije;
using Domain.Modeli;
using Domain.Repozitorijumi;
using Domain.Servisi;

namespace Loger_Bloger.Servisi.Skladistenje
{
    public class DistribucioniCentarServis : ISkladistenjeServis
    {
        private readonly ISkladistaRepozitorijum _skladistaRepozitorijum;
        private readonly IAmbalazaRepozitorijum _ambalazaRepozitorijum;
        private readonly ILoggerServis _logger;


        public DistribucioniCentarServis(ISkladistaRepozitorijum skladistaRepozitorijum, IAmbalazaRepozitorijum ambalazaRepozitorijum, ILoggerServis logger)
        {
            _skladistaRepozitorijum = skladistaRepozitorijum;
            _ambalazaRepozitorijum = ambalazaRepozitorijum;
            _logger = logger;
        }

        public async Task<List<Ambalaza>> PosaljiAmbalazeProdaji(int brojZaSlanje)
        {
            _logger.LogInfo($"[Distribucioni] Slanje ambalaža prodaji: traženo={brojZaSlanje} (limit=3, delay=0.5s).");

            int brojZaistaZaSlanje = Math.Min(brojZaSlanje, 3);
            var sveAmbalaze = _ambalazaRepozitorijum.VratiSve();
            var svaSkladista = _skladistaRepozitorijum.VratiSva();
            var poslateAmbalaze = new List<Ambalaza>();

            var dostupne = sveAmbalaze
                .Where(a => a.Status == StatusAmbalaze.Spakovana)
                .Where(a => svaSkladista.Any(s => s.Id == a.SkladisteId && s.AmbalazeId.Contains(a.Id)))
                .Take(brojZaistaZaSlanje)
                .ToList();

            if (dostupne.Count == 0)
            {
                _logger.LogWarning($"[Distribucioni] Nema dostupnih spakovanih ambalaža (traženo={brojZaSlanje}).");
                return new List<Ambalaza>();
            }

            var diranaSkladistaId = new HashSet<Guid>();

            foreach (var ambalaza in dostupne)
            {
                await Task.Delay(500);

                ambalaza.Status = StatusAmbalaze.Poslata;

                var skladiste = svaSkladista.FirstOrDefault(s => s.Id == ambalaza.SkladisteId);
                if (skladiste != null)
                {
                    skladiste.AmbalazeId.Remove(ambalaza.Id);
                    if (skladiste.TrenutniKapacitet > 0) skladiste.TrenutniKapacitet--;
                    diranaSkladistaId.Add(skladiste.Id);
                }

                poslateAmbalaze.Add(ambalaza);
            }

            if (!_ambalazaRepozitorijum.OznaciKaoPoslate(poslateAmbalaze.Select(a => a.Id).ToList()))
                throw new Exception("Neuspešno čuvanje statusa ambalaža.");

            foreach (var sid in diranaSkladistaId)
            {
                var s = svaSkladista.FirstOrDefault(x => x.Id == sid);
                if (s != null && !_skladistaRepozitorijum.Izmeni(s))
                    throw new Exception("Neuspešno čuvanje promena skladišta.");
            }

            _logger.LogInfo($"[Distribucioni] Poslato ambalaža: {poslateAmbalaze.Count}.");
            return poslateAmbalaze;
        }
    }
}
