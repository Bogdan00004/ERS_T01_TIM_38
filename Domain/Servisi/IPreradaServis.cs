using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Modeli;

namespace Domain.Servisi
{
    public interface IPreradaServis
    {
        List<Parfem> PreradiBiljke(string naziv, int brojBocica, int zapreminaPoBocici);
        List<Parfem> VratiSveParfeme();
    }
}