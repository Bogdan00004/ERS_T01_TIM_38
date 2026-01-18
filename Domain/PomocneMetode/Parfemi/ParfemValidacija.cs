using Domain.Konstante;

namespace Domain.PomocneMetode.Parfemi
{
    public static class ParfemValidacija
    {
        public static bool ValidirajZapreminu(int zapreminaMl)
        {
            return zapreminaMl == PreradaKonstante.ZAPREMINA_150 || zapreminaMl == PreradaKonstante.ZAPREMINA_250;
        }
    }
}
