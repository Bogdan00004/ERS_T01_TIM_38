using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Modeli
{
    public class Skladiste
    {
        public Guid Id { get; set; }
        public string Naziv { get; set; }
        public int MaxKapacitet { get; set; }
        public int TrenutniKapacitet { get; set; }

        public Skladiste()
        {
            Id = Guid.NewGuid();
            TrenutniKapacitet = 0;
        }
    }
}
