using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Modeli;

namespace Services.Interfaces
{
    public interface IProizvodnjaServis
    {
        void PosadiNovuBiljku(string naziv, string latinskiNaziv, string zemljaPorekla);
        void PromeniJacinuUlja(Guid idBiljke, double procenat);
        List<Biljka> UberiBiljke(string naziv, int kolicina);
    }
}
