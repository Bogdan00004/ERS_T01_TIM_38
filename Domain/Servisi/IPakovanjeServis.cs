namespace Domain.Servisi
{
    public interface IPakovanjeServis
    {
        void SpakujParfeme(string nazivParfema, string tip, decimal cena, int brojBocica, int zapreminaPoBocici, Guid skladisteId);
        void PosaljiAmbalazuUSkladiste(Guid skladisteId);
        void ObezbediAmbalazuUSkladistu(string nazivParfema, string tip, decimal cena, int brojBocica, int zapreminaPoBocici, Guid skladisteId);
    }
}
