using Domain.Modeli;

namespace Domain.Repozitorijumi
{
    public interface ISkladistaRepozitorijum
    {
        void Dodaj(Skladiste skladiste);
        Skladiste? NadjiPoId(Guid id);
        List<Skladiste> VratiSva();
        void SacuvajPromene();
    }
}
