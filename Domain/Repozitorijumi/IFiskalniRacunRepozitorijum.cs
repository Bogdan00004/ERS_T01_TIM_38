using Domain.Modeli;

namespace Domain.Repozitorijumi;

public interface IFiskalniRacunRepozitorijum
{
    void Dodaj(FiskalniRacun racun);
    List<FiskalniRacun> VratiSve();
    FiskalniRacun? NadjiPoId(Guid id);
    void Obrisi(Guid id);
    void SacuvajPromene();
}
