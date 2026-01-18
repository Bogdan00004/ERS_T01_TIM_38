using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Modeli;


namespace Domain.PomocneMetode.Prodaja
{
    public static class KatalogHelper
    {
        public static KatalogStavka? NadjiStavku(List<KatalogStavka> katalog, Parfem p)
        {
            var cenaNorm = decimal.Round(p.Cena, 2);

            return katalog.FirstOrDefault(k =>k.Naziv == p.Naziv && k.Tip == p.Tip &&k.NetoKolicina == p.NetoKolicina && decimal.Round(k.Cena, 2) == cenaNorm);
        }
    }
}
