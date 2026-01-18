using Domain.Modeli;

namespace Domain.Repozitorijumi
{
    public interface ISkladistaRepozitorijum
    {
        bool Dodaj(Skladiste skladiste);
        bool Obrisi(Guid id);
        bool Izmeni(Skladiste skladiste);
        Skladiste NadjiPoId(Guid id);
        List<Skladiste> VratiSva();
        bool UkloniAmbalazuIzSkladista(Guid skladisteId, Guid ambalazaId);

    }
}
