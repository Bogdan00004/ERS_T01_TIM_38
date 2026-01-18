using Domain.Konstante;

namespace Domain.PomocneMetode.Biljke
{
    public static class BiljkaRacunanje
    {
        public static int IzracunajPotrebneBiljke(int brojBocica, int zapreminaMl)
        {
            if (brojBocica <= 0) throw new ArgumentOutOfRangeException(nameof(brojBocica));
            if (zapreminaMl <= 0) throw new ArgumentOutOfRangeException(nameof(zapreminaMl));

            int ukupnoMl = brojBocica * zapreminaMl;
            return (int)Math.Ceiling(ukupnoMl / (double)PreradaKonstante.ML_PO_BILJCI);
        }
    }
}
