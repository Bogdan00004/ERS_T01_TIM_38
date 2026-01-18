using Domain.Modeli;
using Domain.PomocneMetode.Biljke;
using Domain.Repozitorijumi;
using Domain.Servisi;

namespace Services.Implementacije
{
    public class ProizvodnjaServis : IProizvodnjaServis
    {
        private readonly IBiljkeRepozitorijum _biljkeRepozitorijum;
        private readonly ILoggerServis _logger;
        public ProizvodnjaServis(IBiljkeRepozitorijum biljkeRepozitorijum, ILoggerServis logger)
        {
            _biljkeRepozitorijum = biljkeRepozitorijum;
            _logger = logger;
        }
        public Biljka PosadiNovuBiljku(string naziv, string latinskiNaziv, string zemljaPorekla)
        {
            try
            {
                double jacina = BiljkaGenerator.GenerisiJacinuUlja();
                var novaBiljka = new Biljka(naziv, jacina, latinskiNaziv, zemljaPorekla, StanjeBiljke.Posadjena);

                if (!_biljkeRepozitorijum.Dodaj(novaBiljka))
                {
                    _logger.LogError("[Proizvodnja] Neuspešno dodavanje biljke u bazu.");
                    return new Biljka();
                }

                _logger.LogInfo($"[Proizvodnja] Posađena biljka: {novaBiljka.Naziv}, ulja={novaBiljka.JacinaAromaticnihUlja}, id={novaBiljka.Id}");
                return novaBiljka;
            }
            catch
            {
                _logger.LogError("[Proizvodnja] Greška u PosadiNovuBiljku.");
                return new Biljka();
            }
        }
        public bool PromeniJacinuUlja(Guid idBiljke, double procenat)
        {
            try
            {
                var biljka = _biljkeRepozitorijum.NadjiPoId(idBiljke);

                if (biljka.Id == Guid.Empty)
                {
                    _logger.LogWarning("[Proizvodnja] Biljka nije pronađena.");
                    return false;
                }

                if (procenat < 0) procenat = 0;
                if (procenat > 100) procenat = 100;

                double faktor = procenat / 100.0;
                var stara = biljka.JacinaAromaticnihUlja;
                biljka.JacinaAromaticnihUlja = Math.Round(stara * faktor, 2);

                if (!_biljkeRepozitorijum.Izmeni(biljka))
                {
                    _logger.LogError($"[Proizvodnja] Neuspešno čuvanje jačine ulja. id={biljka.Id}");
                    return false;
                }

                _logger.LogInfo($"[Proizvodnja] Promenjena jačina ulja: id={biljka.Id}, {stara} -> {biljka.JacinaAromaticnihUlja} (procenat={procenat}%)");
                return true;
            }
            catch
            {
                _logger.LogError("[Proizvodnja] Greška u PromeniJacinuUlja.");
                return false;
            }
        }
        public List<Biljka> UberiBiljke(string naziv, int kolicina)
        {
            try
            {
                if (kolicina <= 0) return new List<Biljka>();
                var zaBerbu = _biljkeRepozitorijum.VratiPoNazivuIStanji(naziv, StanjeBiljke.Posadjena, kolicina);

                if (zaBerbu.Count == 0)
                {
                    _logger.LogWarning($"[Proizvodnja] Nema posađenih biljaka za berbu: naziv={naziv}");
                    return new List<Biljka>();
                }

                foreach (var biljka in zaBerbu)
                {
                    biljka.Stanje = StanjeBiljke.Ubrana;

                    if (!_biljkeRepozitorijum.Izmeni(biljka))
                    {
                        _logger.LogError($"[Proizvodnja] Neuspešno ažuriranje biljke. id={biljka.Id}");
                        return new List<Biljka>();
                    }
                }

                _logger.LogInfo($"[Proizvodnja] Ubrano: naziv={naziv}, kolicina={zaBerbu.Count}");
                return zaBerbu;
            }
            catch
            {
                _logger.LogError("[Proizvodnja] Greška u UberiBiljke.");
                return new List<Biljka>();
            }
        }
    }
}
