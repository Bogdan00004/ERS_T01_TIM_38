using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enumeracije;

namespace Domain.Modeli
{
    public class Ambalaza
    {
        public Guid Id { get; set; }
        public string Naziv { get; set; } = "";
        public string AdresaPosiljaoca { get; set; } = "";
        public Guid SkladisteId { get; set; }
        public List<Guid>ParfemiId { get; set; }
        public StatusAmbalaze Status { get; set; }

        public Ambalaza()
        {
            Id = Guid.NewGuid();
            ParfemiId = new List<Guid>();
            Status = StatusAmbalaze.Spakovana;
        }
    }
}
