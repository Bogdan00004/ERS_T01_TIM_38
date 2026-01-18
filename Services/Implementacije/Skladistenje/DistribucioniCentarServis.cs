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

            if (brojZaSlanje <= 0)
                return new List<Ambalaza>();

            int brojZaistaZaSlanje = Math.Min(brojZaSlanje, 3);

            var svaSkladista = _skladistaRepozitorijum.VratiSva();
            var dostupne = _ambalazaRepozitorijum.VratiSpakovaneKojeSuUSkladistu(svaSkladista, brojZaistaZaSlanje);

            if (dostupne.Count == 0)
            {
                _logger.LogWarning($"[Distribucioni] Nema dostupnih spakovanih ambalaža (traženo={brojZaSlanje}).");
                return new List<Ambalaza>();
            }

            var poslateAmbalaze = new List<Ambalaza>();
            var ids = new List<Guid>();

            foreach (var ambalaza in dostupne)
            {
                await Task.Delay(500);

                ambalaza.Status = StatusAmbalaze.Poslata;

                // repo skida ambalazu iz skladista i cuva
                bool okSkladiste = _skladistaRepozitorijum.UkloniAmbalazuIzSkladista(ambalaza.SkladisteId, ambalaza.Id);
                if (!okSkladiste)
                {
                    _logger.LogWarning($"[Distribucioni] Neuspeh pri ažuriranju skladišta. skladisteId={ambalaza.SkladisteId}, ambalazaId={ambalaza.Id}");
                    continue;
                }

                poslateAmbalaze.Add(ambalaza);
                ids.Add(ambalaza.Id);
            }

            // snimi statuse ambalaza
            bool okAmb = _ambalazaRepozitorijum.OznaciKaoPoslate(ids);
            if (!okAmb)
            {
                _logger.LogError("[Distribucioni] Neuspešno čuvanje statusa ambalaža.");
                return new List<Ambalaza>();
            }

            _logger.LogInfo($"[Distribucioni] Poslato ambalaža: {poslateAmbalaze.Count}.");
            return poslateAmbalaze;
        }
    }
}
