using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Domain.Modeli;

namespace Domain.Repozitorijumi;

public interface IFiskalniRacunRepozitorijum
{
    void Dodaj(FiskalniRacun racun);
    List<FiskalniRacun> VratiSve();
    void SacuvajPromene();
}
