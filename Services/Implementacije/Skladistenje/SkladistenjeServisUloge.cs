using Domain.Enumeracije;
using Domain.Servisi;

namespace Loger_Bloger.Servisi.Skladistenje
{
    public class SkladistenjeServisUloge : ISkladistenjeServisUloge
    {
        private readonly ISkladistenjeServis _magacinskiServis;
        private readonly ISkladistenjeServis _distribucioniServis;
        private readonly ILoggerServis _logger;

        public SkladistenjeServisUloge(MagacinskiCentarServis magacinskiServis, DistribucioniCentarServis distribucioniServis, ILoggerServis logger)
        {
            _magacinskiServis = magacinskiServis;
            _distribucioniServis = distribucioniServis;
            _logger = logger;
        }

        public ISkladistenjeServis KreirajServis(TipKorisnika uloga)
        {
            if (uloga == TipKorisnika.Prodavac)
                return _magacinskiServis;

            if (uloga == TipKorisnika.MenadzerProdaje)
                return _distribucioniServis;

            // nema throw
            _logger.LogError($"[SkladistenjeServisUloge] Nepoznata uloga korisnika: {uloga}. Vracam magacinski servis kao podrazumevani.");
            return _magacinskiServis;
        }
    }
}
