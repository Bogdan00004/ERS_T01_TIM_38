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
