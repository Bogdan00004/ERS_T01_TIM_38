using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Modeli;

public enum TipProdaje { Maloprodaja, Veleprodaja }
public enum NacinPlacanja { Gotovina, UplataNaRacun, Kartica }

public class FiskalnaStavka
{
    public Guid ParfemId { get; set; }
    public string NazivParfema { get; set; } = "";
    public int Kolicina { get; set; }

    public decimal CenaPoKomadu { get; set; }
    public decimal Ukupno => CenaPoKomadu * Kolicina;
}

public class FiskalniRacun
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime DatumVreme { get; set; } = DateTime.Now;

    public TipProdaje TipProdaje { get; set; }
    public NacinPlacanja NacinPlacanja { get; set; }

    public List<FiskalnaStavka> Stavke { get; set; } = new();
    public decimal IznosZaNaplatu => Stavke.Sum(s => s.Ukupno);
}
