using Domain.Modeli;

namespace Domain.Repozitorijumi
{
    public interface IBiljkeRepozitorijum
    {
        List<Biljka> SveBiljke();
        bool Dodaj(Biljka biljka);
        bool Izmeni(Biljka biljka);
        Biljka NadjiPoId(Guid id);
        bool Obrisi(Guid id);
    }
}