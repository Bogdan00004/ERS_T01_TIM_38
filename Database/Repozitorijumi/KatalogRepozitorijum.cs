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
            var skladista = _skladistaRepo.VratiSva();
            var ambalazeUSkladistu = _ambalazeRepo.VratiSpakovaneKojeSuUSkladistu(skladista, int.MaxValue);
            var mapaParfema = ProdajaPomocneMetode.NapraviMapuParfema(_parfemiRepo);

            var kolicine = new Dictionary<string, int>();
            var predstavnik = new Dictionary<string, Parfem>();

            for (int a = 0; a < ambalazeUSkladistu.Count; a++)
            {
                var amb = ambalazeUSkladistu[a];

                for (int j = 0; j < amb.ParfemiId.Count; j++)
                {
                    var parfemId = amb.ParfemiId[j];
                    if (!mapaParfema.ContainsKey(parfemId)) continue;

                    var p = mapaParfema[parfemId];
                    var key = ProdajaKljucHelper.NapraviKljuc(p);

                    if (!kolicine.ContainsKey(key)) kolicine[key] = 0;
                    kolicine[key]++;

                    if (!predstavnik.ContainsKey(key)) predstavnik[key] = p;
                }
            }

            var katalog = new List<KatalogStavka>();
            foreach (var kv in kolicine)
            {
                var p = predstavnik[kv.Key];
                katalog.Add(new KatalogStavka(p.Id, p.Naziv, p.Tip, p.NetoKolicina, p.Cena, kv.Value));
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

                for (int j = 0; j < amb.ParfemiId.Count; j++)
                {
                    var parfemId = amb.ParfemiId[j];
                    if (!mapaParfema.ContainsKey(parfemId)) continue;

                    var p = mapaParfema[parfemId];
                    if (p.Naziv == naziv && p.Tip == tip && p.NetoKolicina == netoKolicina)
                        raspolozivo++;
                }
            }

            return raspolozivo;
        }
    }

}
