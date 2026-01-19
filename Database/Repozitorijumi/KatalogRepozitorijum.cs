using Domain.Modeli;
using Domain.PomocneMetode.Prodaja;
using Domain.Repozitorijumi;

namespace Database.Repozitorijumi
{
    public class KatalogRepozitorijum : IKatalogRepozitorijum
    {
        private readonly IParfemRepozitorijum _parfemiRepo;
        private readonly IAmbalazaRepozitorijum _ambalazeRepo;
        private readonly ISkladistaRepozitorijum _skladistaRepo;

        public KatalogRepozitorijum(
            IParfemRepozitorijum parfemiRepo,
            IAmbalazaRepozitorijum ambalazeRepo,
            ISkladistaRepozitorijum skladistaRepo)
        {
            _parfemiRepo = parfemiRepo;
            _ambalazeRepo = ambalazeRepo;
            _skladistaRepo = skladistaRepo;
        }

        public List<KatalogStavka> VratiKatalogDostupnihParfema()
        {
            var sviParfemi = _parfemiRepo.SviParfemi();

            // Ucitaj sve spakovane ambalaze u skladistima
            var skladista = _skladistaRepo.VratiSva();
            var ambalazeUSkladistu = _ambalazeRepo.VratiSpakovaneKojeSuUSkladistu(skladista, int.MaxValue);

            // Napravi mapu parfema (id -> parfem)
            var mapaParfema = ProdajaPomocneMetode.NapraviMapuParfema(_parfemiRepo);

            // Prebroj raspolozivo po "kljucu" (Naziv|Tip|NetoKolicina)
            // BITNO: ovde racunamo broj BOCA, a ne broj AMBALAZA
            var kolicinePoKljucu = new Dictionary<string, int>();

            for (int a = 0; a < ambalazeUSkladistu.Count; a++)
            {
                var amb = ambalazeUSkladistu[a];
                if (amb == null) continue;
                if (amb.ParfemiId == null) continue;

                for (int j = 0; j < amb.ParfemiId.Count; j++)
                {
                    var parfemId = amb.ParfemiId[j];
                    if (!mapaParfema.ContainsKey(parfemId)) continue;

                    var p = mapaParfema[parfemId];
                    var key = ProdajaKljucHelper.NapraviKljuc(p);

                    if (!kolicinePoKljucu.ContainsKey(key))
                        kolicinePoKljucu[key] = 0;


                    kolicinePoKljucu[key] += 1;
                }
            }

            // Popuni katalog SVIM parfemima (0 ako nema na stanju)
            var katalog = new List<KatalogStavka>();
            var dodat = new HashSet<string>();

            for (int i = 0; i < sviParfemi.Count; i++)
            {
                var p = sviParfemi[i];
                if (p == null) continue;
                if (p.Id == Guid.Empty) continue;

                var key = ProdajaKljucHelper.NapraviKljuc(p);

                if (dodat.Contains(key))
                    continue;

                dodat.Add(key);

                int raspolozivo = 0;
                if (kolicinePoKljucu.ContainsKey(key))
                    raspolozivo = kolicinePoKljucu[key];

                // U katalog stavljamo ID "predstavnika" prvog 
                katalog.Add(new KatalogStavka(p.Id, p.Naziv, p.Tip, p.NetoKolicina, p.Cena, raspolozivo));
            }

            katalog.Sort((x, y) =>
            {
                int c = string.Compare(x.Naziv, y.Naziv, StringComparison.Ordinal);
                if (c != 0) return c;

                c = string.Compare(x.Tip, y.Tip, StringComparison.Ordinal);
                if (c != 0) return c;

                return x.NetoKolicina.CompareTo(y.NetoKolicina);
            });

            return katalog;
        }

        public int VratiRaspolozivo(string naziv, string tip, int netoKolicina)
        {
            var skladista = _skladistaRepo.VratiSva();
            var ambalazeUSkladistu = _ambalazeRepo.VratiSpakovaneKojeSuUSkladistu(skladista, int.MaxValue);
            var mapaParfema = ProdajaPomocneMetode.NapraviMapuParfema(_parfemiRepo);

            int raspolozivo = 0;

            for (int a = 0; a < ambalazeUSkladistu.Count; a++)
            {
                var amb = ambalazeUSkladistu[a];
                if (amb == null) continue;
                if (amb.ParfemiId == null) continue;

                for (int j = 0; j < amb.ParfemiId.Count; j++)
                {
                    var parfemId = amb.ParfemiId[j];
                    if (!mapaParfema.ContainsKey(parfemId)) continue;

                    var p = mapaParfema[parfemId];
                    if (p == null) continue;
                    // Uporedjujemo po "proizvodu" (naziv, tip, neto)
                    if ((p.Naziv ?? "").Trim().Equals((naziv ?? "").Trim(), StringComparison.OrdinalIgnoreCase) && (p.Tip ?? "").Trim().Equals((tip ?? "").Trim(), StringComparison.OrdinalIgnoreCase) && p.NetoKolicina == netoKolicina)
                    {
                        raspolozivo++;
                    }
                }
            }
            return raspolozivo;
        }
    }

}
