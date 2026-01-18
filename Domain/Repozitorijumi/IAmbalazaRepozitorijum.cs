using Domain.Modeli;

namespace Domain.Repozitorijumi
{
    public interface IAmbalazaRepozitorijum
    {
        bool Dodaj(Ambalaza ambalaza);
        bool Izmeni(Ambalaza ambalaza);
        bool Obrisi(Guid id);
        List<Ambalaza> VratiSve();
        Ambalaza NadjiPoId(Guid id);      
        List<Ambalaza> VratiSpakovaneAmbalaze();
        bool OznaciKaoPoslate(List<Guid> ambalazeId);
    }
}
