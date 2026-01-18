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
        private readonly ILoggerServis _logger;

        public MagacinskiCentarServis(ISkladistaRepozitorijum skladistaRepozitorijum, IAmbalazaRepozitorijum ambalazaRepozitorijum, ILoggerServis logger)
        {
            _skladistaRepozitorijum = skladistaRepozitorijum;
            _ambalazaRepozitorijum = ambalazaRepozitorijum;
            _logger = logger;
        }

        public async Task<List<Ambalaza>> PosaljiAmbalazeProdaji(int brojZaSlanje)
        {
            var sveAmbalaze = _ambalazaRepozitorijum.VratiSve();
            var poslateAmbalaze = new List<Ambalaza>();
            var svaSkladista = _skladistaRepozitorijum.VratiSva();

            _logger.LogInfo($"[Magacinski] Slanje ambalaža: traženo={brojZaSlanje}.");

            // Magacinski centar šalje po 1 ambalažu
            int brojZaistaZaSlanje = Math.Min(brojZaSlanje, 1);

            var dostupne = sveAmbalaze.Where(a => a.Status == StatusAmbalaze.Spakovana)
                .Where(a => svaSkladista.Any(s => s.Id == a.SkladisteId && s.AmbalazeId.Contains(a.Id)))
                .Take(brojZaistaZaSlanje)
                .ToList();

            if (dostupne.Count == 0)
            {
                _logger.LogWarning($"[Magacinski] Nema dostupnih spakovanih ambalaža (traženo={brojZaSlanje}).");
                return new List<Ambalaza>();
            }

            foreach (var ambalaza in dostupne)
            {
                await Task.Delay(2500); // 2.5 sekunde po ambalaži
                ambalaza.Status = StatusAmbalaze.Poslata;
                var skladiste = svaSkladista.FirstOrDefault(s => s.Id == ambalaza.SkladisteId);
                if (skladiste != null)
                {
                    skladiste.AmbalazeId.Remove(ambalaza.Id);
                    if (skladiste.TrenutniKapacitet > 0) skladiste.TrenutniKapacitet--;
                }
                poslateAmbalaze.Add(ambalaza);
            }

            _ambalazaRepozitorijum.SacuvajPromene();
            _skladistaRepozitorijum.SacuvajPromene();

            _logger.LogInfo($"[Magacinski] Poslato ambalaža: {poslateAmbalaze.Count}.");
            return poslateAmbalaze;
        }
    }
}
