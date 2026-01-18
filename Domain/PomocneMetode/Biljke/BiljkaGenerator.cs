using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Konstante;

namespace Domain.PomocneMetode.Biljke
{
    public static class BiljkaGenerator
    {
        private static readonly Random _rng = new Random();

        public static double GenerisiJacinuUlja()
        {
            double v = BiljkaKonstante.MIN_JACINA_ULJA + _rng.NextDouble() * (BiljkaKonstante.MAX_JACINA_ULJA - BiljkaKonstante.MIN_JACINA_ULJA);

            return Math.Round(v, 2);
        }
        public static double GenerisiDodatakZaJacinu()
        {
            // dodatak 0.00 – 1.00, za balansiranje u preradi
            return Math.Round(_rng.NextDouble(), 2);
        }
    }

}
