using Domain.Modeli;

namespace Domain.Repozitorijumi
{
    public interface IAmbalazaRepozitorijum
    {
        bool Dodaj(Ambalaza ambalaza);
        List<Ambalaza> VratiSve();
        Ambalaza NadjiPoId(Guid id);
        bool Obrisi(Guid id);
        bool SacuvajPromene();
        List<Ambalaza> VratiSpakovaneAmbalaze();
    }
}
