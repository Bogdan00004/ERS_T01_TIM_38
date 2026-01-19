using Domain.Enumeracije;
namespace Domain.Modeli;

public class FiskalnaStavka
{
    public Guid ParfemId { get; set; }
    public string NazivParfema { get; set; } = "";
    public int Kolicina { get; set; }

    public decimal CenaPoKomadu { get; set; }
    public decimal Ukupno => CenaPoKomadu * Kolicina;
    public FiskalnaStavka()
    {
    }
    public FiskalnaStavka(Guid parfemId, string nazivParfema, int kolicina, decimal cenaPoKomadu)
    {
        ParfemId = parfemId;
        NazivParfema = nazivParfema;
        Kolicina = kolicina;
        CenaPoKomadu = cenaPoKomadu;
    }
    public bool Isporuceno { get; set; }

    public string Napomena { get; set; } = "";

}

public class FiskalniRacun
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime DatumVreme { get; set; } = DateTime.Now;

    public TipProdaje TipProdaje { get; set; }
    public NacinPlacanja NacinPlacanja { get; set; }

    public List<FiskalnaStavka> Stavke { get; set; } = new();
    public decimal IznosZaNaplatu => Stavke.Sum(s => s.Ukupno);
    public FiskalniRacun()
    {
        Id = Guid.Empty;
        Stavke = new List<FiskalnaStavka>();
    }
    public FiskalniRacun(TipProdaje tipProdaje, NacinPlacanja nacinPlacanja, List<FiskalnaStavka>? stavke = null)
    {
        Id = Guid.NewGuid();
        DatumVreme = DateTime.Now;
        TipProdaje = tipProdaje;
        NacinPlacanja = nacinPlacanja;
        Stavke = stavke ?? new List<FiskalnaStavka>();
    }
}
