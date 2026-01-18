using Domain.Enumeracije;
using Domain.Modeli;
using Domain.Repozitorijumi;
using Domain.Servisi;

namespace Domain.PomocneMetode.Prodaja
{
    public static class ProdajaPomocneMetode
    {
        public const int MAX_POKUSAJA_PREUZIMANJA = 50;
        public const int MAX_AMBALAZA_PO_TURI = 3;

        public static Dictionary<Guid, Parfem> NapraviMapuParfema(IParfemRepozitorijum parfemiRepo)
        {
            var sviParfemi = parfemiRepo.SviParfemi();
            var mapa = new Dictionary<Guid, Parfem>();

            for (int i = 0; i < sviParfemi.Count; i++)
            {
                var p = sviParfemi[i];
                if (!mapa.ContainsKey(p.Id))
                    mapa[p.Id] = p;
            }

            return mapa;
        }

        public static int PrebrojSkupljeno(List<Ambalaza> ambalaze, Dictionary<Guid, Parfem> mapaParfema, string trazeniKey)
        {
            int skupljeno = 0;

            for (int a = 0; a < ambalaze.Count; a++)
            {
                var amb = ambalaze[a];

                for (int j = 0; j < amb.ParfemiId.Count; j++)
                {
                    var pid = amb.ParfemiId[j];
                    if (!mapaParfema.ContainsKey(pid)) continue;

                    var p = mapaParfema[pid];
                    if (ProdajaKljucHelper.NapraviKljuc(p) == trazeniKey)
                        skupljeno++;
                }
            }

            return skupljeno;
        }

        public static Skladiste NadjiNajboljeSkladiste(ISkladistaRepozitorijum skladistaRepo)
        {
            var skladista = skladistaRepo.VratiSva();
            Skladiste ciljno = new Skladiste();
            int najboljiVisak = -1;

            for (int i = 0; i < skladista.Count; i++)
            {
                var s = skladista[i];
                int slobodno = s.MaxKapacitet - s.TrenutniKapacitet;
                if (slobodno > 0 && slobodno > najboljiVisak)
                {
                    najboljiVisak = slobodno;
                    ciljno = s;
                }
            }

            return ciljno;
        }

        public static bool SkiniParfeme(List<Ambalaza> ambalaze, int kolicinaBocica, Dictionary<Guid, Parfem> mapaParfema, string trazeniKey, ILoggerServis logger)
        {
            int preostalo = kolicinaBocica;

            for (int a = 0; a < ambalaze.Count && preostalo > 0; a++)
            {
                var amb = ambalaze[a];
                var zaUkloniti = new List<Guid>();

                for (int j = 0; j < amb.ParfemiId.Count && preostalo > 0; j++)
                {
                    var pid = amb.ParfemiId[j];
                    if (!mapaParfema.ContainsKey(pid)) continue;

                    var p = mapaParfema[pid];
                    if (ProdajaKljucHelper.NapraviKljuc(p) == trazeniKey)
                    {
                        zaUkloniti.Add(pid);
                        preostalo--;
                    }
                }

                for (int x = 0; x < zaUkloniti.Count; x++)
                    amb.ParfemiId.Remove(zaUkloniti[x]);
            }

            if (preostalo > 0)
            {
                logger.LogError("[Prodaja] Logička greška: nije skinuto dovoljno parfema.");
                return false;
            }

            return true;
        }

        public static bool SacuvajAmbalaze(IAmbalazaRepozitorijum ambalazeRepo, List<Ambalaza> ambalaze, string porukaGreske, ILoggerServis logger)
        {
            for (int i = 0; i < ambalaze.Count; i++)
            {
                if (!ambalazeRepo.Izmeni(ambalaze[i]))
                {
                    logger.LogError(porukaGreske);
                    return false;
                }
            }
            return true;
        }

        public static bool VratiUSkladisteAkoMoze(ISkladistaRepozitorijum skladistaRepo, Skladiste skladiste, Ambalaza amb, ILoggerServis logger)
        {
            if (skladiste.Id == Guid.Empty) return true;

            if (!skladiste.AmbalazeId.Contains(amb.Id))
            {
                if (skladiste.TrenutniKapacitet < skladiste.MaxKapacitet)
                {
                    skladiste.AmbalazeId.Add(amb.Id);
                    skladiste.TrenutniKapacitet++;
                }
                else
                {
                    logger.LogWarning($"[Prodaja] Ne mogu da vratim ambalazu u skladiste (popunjeno). ambalazaId={amb.Id}, skladisteId={skladiste.Id}");
                }
            }

            if (!skladistaRepo.Izmeni(skladiste))
            {
                logger.LogError("[Prodaja] Neuspešno čuvanje promena skladišta.");
                return false;
            }

            return true;
        }

        public static bool ObradiStatusIVracanje(ISkladistaRepozitorijum skladistaRepo, IAmbalazaRepozitorijum ambalazeRepo, List<Ambalaza> ambalaze, ILoggerServis logger)
        {
            for (int i = 0; i < ambalaze.Count; i++)
            {
                var amb = ambalaze[i];

                if (amb.ParfemiId.Count > 0)
                {
                    var skladiste = skladistaRepo.NadjiPoId(amb.SkladisteId);
                    if (!VratiUSkladisteAkoMoze(skladistaRepo, skladiste, amb, logger))
                        return false;

                    amb.Status = StatusAmbalaze.Spakovana;
                }
                else
                {
                    amb.Status = StatusAmbalaze.Poslata;
                }

                if (!ambalazeRepo.Izmeni(amb))
                {
                    logger.LogError("[Prodaja] Neuspešno čuvanje promena ambalaža (status).");
                    return false;
                }
            }

            return true;
        }

        public static FiskalniRacun KreirajRacun(Parfem izabrani, int kolicinaBocica, TipProdaje tipProdaje, NacinPlacanja nacinPlacanja)
        {
            var racun = new FiskalniRacun(tipProdaje, nacinPlacanja);
            racun.Stavke.Add(new FiskalnaStavka(
                izabrani.Id,
                $"{izabrani.Naziv} ({izabrani.Tip}, {izabrani.NetoKolicina}ml)",
                kolicinaBocica,
                izabrani.Cena));
            return racun;
        }
    }
}
