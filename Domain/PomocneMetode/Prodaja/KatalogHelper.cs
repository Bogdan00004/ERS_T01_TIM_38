using Domain.Modeli;


namespace Domain.PomocneMetode.Prodaja
{
    public static class KatalogHelper
    {
        public static KatalogStavka? NadjiStavku(List<KatalogStavka> katalog, Parfem p)
        {
            if (katalog == null || p == null) return null;

            var cenaNorm = decimal.Round(p.Cena, 2);

            for (int i = 0; i < katalog.Count; i++)
            {
                var k = katalog[i];
                if (k == null) continue;

                if (k.Naziv == p.Naziv &&
                    k.Tip == p.Tip &&
                    k.NetoKolicina == p.NetoKolicina &&
                    decimal.Round(k.Cena, 2) == cenaNorm)
                {
                    return k;
                }
            }

            return null;
        }
    }
}
