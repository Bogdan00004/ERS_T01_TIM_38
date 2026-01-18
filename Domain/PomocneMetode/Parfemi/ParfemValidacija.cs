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
