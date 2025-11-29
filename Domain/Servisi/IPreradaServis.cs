using Domain.Modeli;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Servisi
{
    public interface IPreradaServis
    {
        List<Parfem> PreradiBiljke(string naziv, int brojBocica, double zapreminaPoBocici);
    }
}
