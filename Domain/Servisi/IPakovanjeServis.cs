using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enumeracije;
using Domain.Modeli;

namespace Domain.Servisi
{
    public interface IPakovanjeServis
    {
        void SpakujParfeme(string nazivParfema, string tip, decimal cena, int brojBocica, int zapreminaPoBocici, Guid skladisteId);
        void PosaljiAmbalazuUSkladiste(Guid skladisteId);
        void ObezbediAmbalazuUSkladistu(string nazivParfema, string tip,decimal cena, int brojBocica, int zapreminaPoBocici, Guid skladisteId);
    }
}
