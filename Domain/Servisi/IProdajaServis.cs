using Domain.Enumeracije;
using Domain.Modeli;

namespace Domain.Servisi
{
    public interface IProdajaServis
    {
        List<KatalogStavka> VratiKatalogDostupnihParfema();
        Task<FiskalniRacun> Prodaj(Guid parfemId, int kolicinaBocica, TipProdaje tipProdaje, NacinPlacanja nacinPlacanja);
        List<FiskalniRacun> VratiSveRacune();

    }
}
