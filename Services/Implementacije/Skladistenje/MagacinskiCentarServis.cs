using Domain.Enumeracije;
using Domain.Modeli;
using Domain.Repozitorijumi;
using Domain.Servisi;

namespace Loger_Bloger.Servisi.Skladistenje
{
    public class MagacinskiCentarServis : ISkladistenjeServis
    {
        private readonly ISkladistaRepozitorijum _skladistaRepozitorijum;
        private readonly IAmbalazaRepozitorijum _ambalazaRepozitorijum;
        private readonly ILoggerServis _logger;

        public MagacinskiCentarServis(ISkladistaRepozitorijum skladistaRepozitorijum, IAmbalazaRepozitorijum ambalazaRepozitorijum, ILoggerServis logger)
        {
            _skladistaRepozitorijum = skladistaRepozitorijum;
            _ambalazaRepozitorijum = ambalazaRepozitorijum;
            _logger = logger;
        }

        public async Task<List<Ambalaza>> PosaljiAmbalazeProdaji(int brojZaSlanje)
        {
            _logger.LogInfo($"[Magacinski] Slanje ambalaža prodaji: traženo={brojZaSlanje}.");

            if (brojZaSlanje <= 0)
                return new List<Ambalaza>();

            int brojZaistaZaSlanje = Math.Min(brojZaSlanje, 1);

            // repo vraća sva skladišta
            var svaSkladista = _skladistaRepozitorijum.VratiSva();

            // repo vraća spakovane ambalaže koje se ZAISTA nalaze u skladištu
            var dostupne = _ambalazaRepozitorijum.VratiSpakovaneKojeSuUSkladistu(svaSkladista, brojZaistaZaSlanje);

            if (dostupne.Count == 0)
            {
                _logger.LogWarning($"[Magacinski] Nema dostupnih spakovanih ambalaža.");
                return new List<Ambalaza>();
            }

            var poslateAmbalaze = new List<Ambalaza>();
            var idsZaOznacavanje = new List<Guid>();

            foreach (var ambalaza in dostupne)
            {
                await Task.Delay(2500);

                ambalaza.Status = StatusAmbalaze.Poslata;

                bool skladisteOk = _skladistaRepozitorijum
                    .UkloniAmbalazuIzSkladista(ambalaza.SkladisteId, ambalaza.Id);

                if (!skladisteOk)
                {
                    _logger.LogWarning($"[Magacinski] Neuspešno ažuriranje skladišta. skladisteId={ambalaza.SkladisteId}, ambalazaId={ambalaza.Id}");
                    continue;
                }

                poslateAmbalaze.Add(ambalaza);
                idsZaOznacavanje.Add(ambalaza.Id);
            }

            if (idsZaOznacavanje.Count == 0)
            {
                _logger.LogWarning("[Magacinski] Nijedna ambalaža nije uspešno poslata.");
                return new List<Ambalaza>();
            }

            bool okAmbalaze = _ambalazaRepozitorijum.OznaciKaoPoslate(idsZaOznacavanje);
            if (!okAmbalaze)
            {
                _logger.LogError("[Magacinski] Neuspešno čuvanje statusa ambalaža.");
                return new List<Ambalaza>();
            }

            _logger.LogInfo($"[Magacinski] Poslato ambalaža: {poslateAmbalaze.Count}.");
            return poslateAmbalaze;
        }
    }
}

