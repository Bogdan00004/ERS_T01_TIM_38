using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Modeli;
using Domain.Repozitorijumi;
using Domain.Servisi;
using Domain.Enumeracije;

namespace Loger_Bloger.Servisi.Skladistenje
{
    public class DistribucioniCentarServis:ISkladistenjeServis
    {
        private readonly ISkladistaRepozitorijum _skladistaRepozitorijum;
        private readonly IAmbalazaRepozitorijum _ambalazaRepozitorijum;
        private readonly ILoggerServis _logger;


        public DistribucioniCentarServis(ISkladistaRepozitorijum skladistaRepozitorijum, IAmbalazaRepozitorijum ambalazaRepozitorijum, ILoggerServis logger)
        {
            _skladistaRepozitorijum = skladistaRepozitorijum;
            _ambalazaRepozitorijum = ambalazaRepozitorijum;
            _logger = logger;
        }

        public async Task<List<Ambalaza>> PosaljiAmbalazeProdaji(int brojZaSlanje)
        {
            var sveAmbalaze = _ambalazaRepozitorijum.VratiSve();
            var poslateAmbalaze = new List<Ambalaza>();
            _logger.LogInfo($"[Distribucioni] Slanje ambalaža: traženo={brojZaSlanje}.");

            // Maksimalno 3 ambalaže po slanju
            int brojZaistaZaSlanje = Math.Min(brojZaSlanje, 3);

            var dostupne = sveAmbalaze.Where(a => a.Status == StatusAmbalaze.Spakovana).Take(brojZaistaZaSlanje).ToList();
            if (dostupne.Count == 0)
            {
                _logger.LogWarning($"[Distribucioni] Nema dostupnih spakovanih ambalaža (traženo={brojZaSlanje}).");
            }

            foreach (var ambalaza in dostupne)
            {
                await Task.Delay(500); // 0.5 sekundi po ambalaži
                ambalaza.Status = StatusAmbalaze.Poslata;
                poslateAmbalaze.Add(ambalaza);
            }

            _ambalazaRepozitorijum.SacuvajPromene();
            _logger.LogInfo($"[Distribucioni] Poslato ambalaža: {poslateAmbalaze.Count}.");
            return poslateAmbalaze;
        }
    }
}
