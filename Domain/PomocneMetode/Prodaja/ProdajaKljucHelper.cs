using Domain.Modeli;

namespace Domain.PomocneMetode.Prodaja
{
    public static class ProdajaKljucHelper
    {
        public static string NapraviKljuc(Parfem p)
        {
            string naziv = (p.Naziv ?? "").Trim().ToUpperInvariant();
            string tip = (p.Tip ?? "").Trim().ToUpperInvariant();
            int ml = p.NetoKolicina;

            return $"{naziv}|{tip}|{ml}";
        }
    }
}
