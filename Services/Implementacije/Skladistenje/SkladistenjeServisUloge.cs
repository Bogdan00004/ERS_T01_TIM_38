using Domain.Enumeracije;
using Domain.Servisi;

namespace Loger_Bloger.Servisi.Skladistenje
{
    public class SkladistenjeServisUloge : ISkladistenjeServisUloge
    {
        private readonly ISkladistenjeServis _magacinskiServis;
        private readonly ISkladistenjeServis _distribucioniServis;

        public SkladistenjeServisUloge(MagacinskiCentarServis magacinskiServis, DistribucioniCentarServis distribucioniServis)
        {
            _magacinskiServis = magacinskiServis;
            _distribucioniServis = distribucioniServis;
        }

        public ISkladistenjeServis KreirajServis(TipKorisnika uloga)
        {
            return uloga switch
            {
                TipKorisnika.Prodavac => _magacinskiServis,
                TipKorisnika.MenadzerProdaje => _distribucioniServis,
                _ => throw new ArgumentException("Nepoznata uloga korisnika.")
            };
        }
    }
}
