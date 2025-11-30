using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enumeracije;
using Domain.Modeli;
using Domain.Repozitorijumi;
using Domain.Servisi;

namespace Loger_Bloger.Servisi.Skladistenje
{
    public class MagacinskiCentarServis:ISkladistenjeServis
    {
        private readonly ISkladistaRepozitorijum _skladistaRepozitorijum;
        private readonly IAmbalazaRepozitorijum _ambalazaRepozitorijum;

        public MagacinskiCentarServis(
            ISkladistaRepozitorijum skladistaRepozitorijum,
            IAmbalazaRepozitorijum ambalazaRepozitorijum)
        {
            _skladistaRepozitorijum = skladistaRepozitorijum;
            _ambalazaRepozitorijum = ambalazaRepozitorijum;
        }

        public async Task<List<Ambalaza>> PosaljiAmbalazeProdaji(int brojZaSlanje)
        {
            var sveAmbalaze = _ambalazaRepozitorijum.VratiSve();
            var poslateAmbalaze = new List<Ambalaza>();

            // Magacinski centar šalje po 1 ambalažu
            int brojZaistaZaSlanje = Math.Min(brojZaSlanje, 1);

            var dostupne = sveAmbalaze.Where(a => a.Status == StatusAmbalaze.Spakovana).Take(brojZaistaZaSlanje).ToList();

            foreach (var ambalaza in dostupne)
            {
                await Task.Delay(2500); // 2.5 sekunde po ambalaži
                ambalaza.Status = StatusAmbalaze.Poslata;
                poslateAmbalaze.Add(ambalaza);
            }

            _ambalazaRepozitorijum.SacuvajPromene();
            return poslateAmbalaze;
        }
    }
}
