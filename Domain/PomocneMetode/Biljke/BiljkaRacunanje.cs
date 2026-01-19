namespace Domain.PomocneMetode.Biljke
{
    public static class BiljkaRacunanje
    {
        public static bool IzracunajPotrebanBrojBiljaka(int brojBocica, int zapreminaBocice, out int potrebanBrojBiljaka)
        {
            potrebanBrojBiljaka = 0;

            if (brojBocica <= 0) return false;
            if (zapreminaBocice != 150 && zapreminaBocice != 250) return false;

            int ukupnoMl = brojBocica * zapreminaBocice;

            potrebanBrojBiljaka = ukupnoMl / 50;
            if (ukupnoMl % 50 != 0)
                potrebanBrojBiljaka++;

            return true;
        }
    }
}
