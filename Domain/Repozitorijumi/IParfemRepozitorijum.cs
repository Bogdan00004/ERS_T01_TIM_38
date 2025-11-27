using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Modeli;

namespace Domain.Repozitorijumi
{
    public interface IParfemRepozitorijum
    {
        void Dodaj(Parfem parfem);
        List<Parfem> SviParfemi();
        Parfem NadjiPoId(Guid id);
    }
}
