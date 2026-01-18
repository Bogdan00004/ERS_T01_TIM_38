namespace Domain.Modeli
{
    public class KatalogStavka
    {
        public Guid ParfemId { get; set; }
        public string Naziv { get; set; } = "";
        public string Tip { get; set; } = "";
        public int NetoKolicina { get; set; }
        public decimal Cena { get; set; }
        public int Raspolozivo { get; set; }

        public KatalogStavka()
        {

        }

        public KatalogStavka(Guid parfemId, string naziv, string tip, int netoKolicina, decimal cena, int raspolozivo)
        {
            ParfemId = parfemId;
            Naziv = naziv;
            Tip = tip;
            NetoKolicina = netoKolicina;
            Cena = cena;
            Raspolozivo = raspolozivo;
        }
    }
}
