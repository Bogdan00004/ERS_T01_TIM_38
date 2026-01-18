using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Konstante;

namespace Domain.PomocneMetode.Parfemi
{
    public static class SerijskiBrojHelper
    {
        public static string GenerisiSerijskiBrojParfema(Guid idParfema)
        {
            return $"{PreradaKonstante.SERIJA_PREFIX}-{PreradaKonstante.SERIJA_GODINA}-{idParfema.ToString().ToUpper()}";
        }
    }
}
