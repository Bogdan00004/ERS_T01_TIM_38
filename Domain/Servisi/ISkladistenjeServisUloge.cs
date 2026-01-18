using Domain.Enumeracije;

namespace Domain.Servisi
{
    public interface ISkladistenjeServisUloge
    {
        ISkladistenjeServis KreirajServis(TipKorisnika uloga);
    }

}
