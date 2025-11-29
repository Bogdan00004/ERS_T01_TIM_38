using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Modeli;

namespace Domain.Repozitorijumi
{
    public interface IBiljkeRepozitorijum
    {
        List<Biljka> SveBiljke();
        void Dodaj(Biljka biljka);
        void Izmeni(Biljka biljka);
        Biljka NadjiPoId(Guid id);
        void Obrisi(Guid id);
    }
}
