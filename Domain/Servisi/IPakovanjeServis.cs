namespace Domain.Servisi
{
    public interface IPakovanjeServis
    {
        bool SpakujParfeme(string nazivParfema, string tip, decimal cena, int brojBocica, int zapreminaPoBocici, Guid skladisteId);
        bool PosaljiAmbalazuUSkladiste(Guid skladisteId);
        bool ObezbediAmbalazuUSkladistu(string nazivParfema, string tip, decimal cena, int brojBocica, int zapreminaPoBocici, Guid skladisteId);
    }
}
