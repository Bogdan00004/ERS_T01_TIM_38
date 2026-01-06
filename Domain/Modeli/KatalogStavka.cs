using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace Domain.Modeli
{
    public class KatalogStavka
    {
        public Guid ParfemId { get; set; }
        public string Naziv { get; set; } = "";
        public string Tip { get; set; } = "";
        public int NetoKolicina { get; set; }
        public decimal Cena { get; set; }
        public int Raspolozivo { get; set; }
    }
}
