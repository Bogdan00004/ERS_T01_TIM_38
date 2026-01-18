using Domain.Modeli;

namespace Domain.PomocneMetode.Prodaja
{
    public static class ProdajaKljucHelper
    {
        public static string NapraviKljuc(Parfem p)
        {
            var cenaNorm = decimal.Round(p.Cena, 2);
            return $"{p.Naziv}|{p.Tip}|{p.NetoKolicina}|{cenaNorm:0.##}";
        }
    }
}
