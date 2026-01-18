namespace Domain.Modeli
{
    public enum StanjeBiljke
    {
        Posadjena,
        Ubrana,
        Preradjena
    }
    public class Biljka
    {
        public Guid Id { get; set; }
        public string Naziv { get; set; } = "";
        public double JacinaAromaticnihUlja { get; set; }
        public string LatinskiNaziv { get; set; } = "";
        public string ZemljaPorekla { get; set; } = "";
        public StanjeBiljke Stanje { get; set; }

        public Biljka()
        {
            Id = Guid.NewGuid();
        }
        public Biljka(string naziv, double jacinaAromaticnihUlja, string latinskiNaziv, string zemljaPorekla, StanjeBiljke stanje)
        {
            Id = Guid.NewGuid();
            Naziv = naziv;
            JacinaAromaticnihUlja = jacinaAromaticnihUlja;
            LatinskiNaziv = latinskiNaziv;
            ZemljaPorekla = zemljaPorekla;
            Stanje = stanje;
        }

    }
}
