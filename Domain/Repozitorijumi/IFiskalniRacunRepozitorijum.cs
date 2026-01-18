using Domain.Modeli;

namespace Domain.Repozitorijumi;

public interface IFiskalniRacunRepozitorijum
{
    bool Dodaj(FiskalniRacun racun);
    List<FiskalniRacun> VratiSve();
    FiskalniRacun NadjiPoId(Guid id);
    bool Obrisi(Guid id);
    bool Izmeni(FiskalniRacun racun);
}
