using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Konstante;

namespace Domain.PomocneMetode.Parfemi
{
    public static class ParfemValidacija
    {
        public static void ValidirajZapreminu(int zapreminaMl)
        {
            if (zapreminaMl != PreradaKonstante.ZAPREMINA_150 &&
                zapreminaMl != PreradaKonstante.ZAPREMINA_250)
            {
                throw new ArgumentException("Zapremina mora biti 150ml ili 250ml.");
            }
        }
    }
}
