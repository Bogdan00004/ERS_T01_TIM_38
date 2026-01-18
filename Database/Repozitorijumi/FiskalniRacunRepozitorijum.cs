using Domain.BazaPodataka;
using Domain.Modeli;
using Domain.Repozitorijumi;

namespace Database.Repozitorijumi
{
    public class FiskalniRacuniRepozitorijum : IFiskalniRacunRepozitorijum
    {
        private readonly IBazaPodataka _baza;

        public FiskalniRacuniRepozitorijum(IBazaPodataka baza)
        {
            _baza = baza;
        }

        public bool Dodaj(FiskalniRacun racun)
        {
            try
            {
                _baza.Tabele.FiskalniRacuni.Add(racun);
                _baza.SacuvajPromene();
                return true;
            }
            catch { return false; }
        }
        public bool Izmeni(FiskalniRacun racun)
        {
            try
            {
                _baza.SacuvajPromene();
                return true;
            }
            catch { return false; }
        }

        public List<FiskalniRacun> VratiSve()
        {
            return _baza.Tabele.FiskalniRacuni;
        }

        public FiskalniRacun NadjiPoId(Guid id)
        {
            var racun = _baza.Tabele.FiskalniRacuni.FirstOrDefault(r => r.Id == id);
            return racun ?? new FiskalniRacun(); // nikad null
        }

        public bool Obrisi(Guid id)
        {
            try
            {
                var racun = _baza.Tabele.FiskalniRacuni.FirstOrDefault(r => r.Id == id);
                if (racun == null)
                    return false;

                _baza.Tabele.FiskalniRacuni.Remove(racun);
                _baza.SacuvajPromene();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

