using Domain.Modeli;

namespace Domain.Repozitorijumi
{
    public interface IParfemRepozitorijum
    {
        bool Dodaj(Parfem parfem);
        bool Izmeni(Parfem parfem);
        bool Obrisi(Guid id);
        List<Parfem> SviParfemi();
        Parfem NadjiPoId(Guid id);
    }
}
