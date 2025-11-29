using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Modeli;

namespace Domain.Repozitorijumi
{
    public interface IAmbalazaRepozitorijum
    {
        void Dodaj(Ambalaza ambalaza);
        List<Ambalaza> VratiSve();
        Ambalaza? NadjiPoId(Guid id);
        void Obrisi(Guid id);
        void SacuvajPromene();
        List<Ambalaza> VratiSpakovaneAmbalaze();
    }
}
