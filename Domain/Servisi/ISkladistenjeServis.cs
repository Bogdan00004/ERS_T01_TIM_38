using Domain.Modeli;

namespace Domain.Servisi
{
    public interface ISkladistenjeServis
    {
        Task<List<Ambalaza>> PosaljiAmbalazeProdaji(int brojZaSlanje);
    }
}
