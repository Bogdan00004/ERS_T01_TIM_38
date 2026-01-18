using Domain.Modeli;

namespace Domain.Servisi
{
    public interface IPreradaServis
    {
        List<Parfem> PreradiBiljke(string naziv, string tip, decimal cena, int brojBocica, int zapreminaPoBocici);
        List<Parfem> VratiSveParfeme();
    }
}