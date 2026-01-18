using Domain.Enumeracije;

namespace Domain.Modeli
{
    public class Ambalaza
    {
        public Guid Id { get; set; }
        public string Naziv { get; set; } = "";
        public string AdresaPosiljaoca { get; set; } = "";
        public Guid SkladisteId { get; set; }
        public List<Guid> ParfemiId { get; set; }
        public StatusAmbalaze Status { get; set; }

        public Ambalaza()
        {
            Id = Guid.NewGuid();
            ParfemiId = new List<Guid>();
            Status = StatusAmbalaze.Spakovana;
        }
        public Ambalaza(string naziv, string adresaPosiljaoca, Guid skladisteId, StatusAmbalaze status = StatusAmbalaze.Spakovana, List<Guid>? parfemiId = null)
        {
            Id = Guid.NewGuid();
            Naziv = naziv;
            AdresaPosiljaoca = adresaPosiljaoca;
            SkladisteId = skladisteId;
            Status = status;
            ParfemiId = parfemiId ?? new List<Guid>();
        }
    }
}
