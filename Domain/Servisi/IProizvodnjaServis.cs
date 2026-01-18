using Domain.Modeli;

namespace Domain.Servisi
{
    public interface IProizvodnjaServis
    {
        Biljka PosadiNovuBiljku(string naziv, string latinskiNaziv, string zemljaPorekla);

        bool PromeniJacinuUlja(Guid idBiljke, double procenat);

        List<Biljka> UberiBiljke(string naziv, int kolicina);
    }
}