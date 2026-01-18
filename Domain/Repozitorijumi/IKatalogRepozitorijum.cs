using Domain.Modeli;

namespace Domain.Repozitorijumi
{
    public interface IKatalogRepozitorijum
    {
        List<KatalogStavka> VratiKatalogDostupnihParfema();
        int VratiRaspolozivo(string naziv, string tip, int netoKolicina);
    }
}
