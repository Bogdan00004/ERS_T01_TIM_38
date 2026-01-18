namespace Domain.Modeli
{
    public class Skladiste
    {
        public Guid Id { get; set; }
        public string Naziv { get; set; } = "";
        public string Lokacija { get; set; } = "";
        public int MaxKapacitet { get; set; }
        public int TrenutniKapacitet { get; set; }
        public List<Guid> AmbalazeId { get; set; }

        public Skladiste()
        {
            TrenutniKapacitet = 0;
            AmbalazeId = new List<Guid>();
            Id = Guid.Empty;
        }
        public Skladiste(string naziv, string lokacija, int maxKapacitet)
        {
            Id = Guid.NewGuid();
            Naziv = naziv;
            Lokacija = lokacija;
            MaxKapacitet = maxKapacitet;
            TrenutniKapacitet = 0;
            AmbalazeId = new List<Guid>();
        }
    }
}
