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
        List<Ambalaza> VratiSpakovaneKojeSuUSkladistu(List<Skladiste> skladista, int maxBroj);
        bool OznaciKaoPoslate(List<Guid> ambalazeId);
    }
}
