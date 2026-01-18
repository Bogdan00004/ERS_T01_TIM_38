using Domain.BazaPodataka;
using Domain.Modeli;
using Domain.PomocneMetode.Biljke;
using Domain.Servisi;


namespace Services.Implementacije
{
    public class ProizvodnjaServis : IProizvodnjaServis
    {
        private readonly IBazaPodataka _baza;
        private readonly ILoggerServis _logger;


        public ProizvodnjaServis(IBazaPodataka baza, ILoggerServis logger)
        {
            _baza = baza;
            _logger = logger;
        }
        public Biljka PosadiNovuBiljku(string naziv, string latinskiNaziv, string zemljaPorekla)
        {
            double jacina = BiljkaGenerator.GenerisiJacinuUlja();

            var novaBiljka = new Biljka(naziv, jacina, latinskiNaziv, zemljaPorekla, StanjeBiljke.Posadjena);


            _baza.Tabele.Biljke.Add(novaBiljka);
            _baza.SacuvajPromene();

            _logger.LogInfo($"[Proizvodnja] Posađena biljka: {novaBiljka.Naziv}, ulja={novaBiljka.JacinaAromaticnihUlja}, id={novaBiljka.Id}");
            return novaBiljka;
        }
        public void PromeniJacinuUlja(Guid idBiljke, double procenat)
        {
            var biljka = _baza.Tabele.Biljke.FirstOrDefault(b => b.Id == idBiljke);

            if (biljka == null)
            {
                _logger.LogError("[Proizvodnja] Greška: Biljka nije pronađena.");
                return;
            }

            if (procenat < 0) procenat = 0;
            if (procenat > 100) procenat = 100;

            double faktor = procenat / 100.0;
            var stara = biljka.JacinaAromaticnihUlja;
            biljka.JacinaAromaticnihUlja = Math.Round(stara * faktor, 2);

            _baza.SacuvajPromene();
            _logger.LogInfo($"[Proizvodnja] Promenjena jačina ulja: id={biljka.Id}, {stara} -> {biljka.JacinaAromaticnihUlja} (procenat={procenat}%)");
        }

        public List<Biljka> UberiBiljke(string naziv, int kolicina)
        {
            if (kolicina <= 0) return new List<Biljka>();

            var zaBerbu = _baza.Tabele.Biljke
                .Where(b => b.Naziv == naziv && b.Stanje == StanjeBiljke.Posadjena)
                .Take(kolicina)
                .ToList();

            foreach (var biljka in zaBerbu)
                biljka.Stanje = StanjeBiljke.Ubrana;

            _baza.SacuvajPromene();
            _logger.LogInfo($"[Proizvodnja] Ubrano: naziv={naziv}, kolicina={zaBerbu.Count}");

            return zaBerbu;
        }
    }
}
