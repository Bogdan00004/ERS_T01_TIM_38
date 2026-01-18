namespace Domain.Modeli
{
    public class Parfem
    {
        public Guid Id { get; set; }
        public string Naziv { get; set; } = "";
        public string Tip { get; set; } = "";
        public int NetoKolicina { get; set; }
        public string SerijskiBroj { get; set; } = "";
        public decimal Cena { get; set; }
        public Guid BiljkaId { get; set; }
        public DateTime RokTrajanja { get; set; }
        public Parfem()
        {
            Id = Guid.NewGuid();
        }

        public Parfem(string naziv, string tip, int netoKolicina, string serijskiBroj, decimal cena, Guid biljkaId, DateTime rokTrajanja)
        {
            Id = Guid.NewGuid();
            Naziv = naziv;
            Tip = tip;
            NetoKolicina = netoKolicina;
            SerijskiBroj = serijskiBroj;
            Cena = cena;
            BiljkaId = biljkaId;
            RokTrajanja = rokTrajanja;
        }
    }
}
