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

        public void Dodaj(FiskalniRacun racun)
        {
            _baza.Tabele.FiskalniRacuni.Add(racun);
        }

        public List<FiskalniRacun> VratiSve()
        {
            return _baza.Tabele.FiskalniRacuni;
        }

        public FiskalniRacun? NadjiPoId(Guid id)
        {
            return _baza.Tabele.FiskalniRacuni.FirstOrDefault(r => r.Id == id);
        }

        public void Obrisi(Guid id)
        {
            var racun = NadjiPoId(id);
            if (racun != null)
                _baza.Tabele.FiskalniRacuni.Remove(racun);
        }

        public void SacuvajPromene()
        {
            _baza.SacuvajPromene();
        }
    }
}

