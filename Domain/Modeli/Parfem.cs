using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Modeli
{
    public class Parfem
    {
        public Guid Id { get; set; }
        public string Naziv {  get; set; }
        public string Tip {  get; set; }
        public int NetoKolicina {  get; set; }
        public string SerijskiBroj {  get; set; }
        public Guid BiljkaId {  get; set; }
        public DateTime RokTrajanja { get; set; }
    }
}
