using Domain.Modeli;

namespace Domain.Repozitorijumi
{
    public interface IParfemRepozitorijum
    {
        void Dodaj(Parfem parfem);
        List<Parfem> SviParfemi();
        Parfem? NadjiPoId(Guid id);
    }
}
