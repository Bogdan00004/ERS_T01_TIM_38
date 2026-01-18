using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.BazaPodataka;
using Newtonsoft.Json;

namespace Database.BazaPodataka
{
    public class JsonBazaPodataka:IBazaPodataka
    {
        private readonly string putanja = "baza.json";
        public TabeleBazaPodataka Tabele { get; set; }

        public JsonBazaPodataka()
        {
            if (File.Exists(putanja))
            {
                string json = File.ReadAllText(putanja);
                Tabele = JsonConvert.DeserializeObject<TabeleBazaPodataka>(json) ?? new TabeleBazaPodataka();
                if(DaLiJeBazaPrazna())
                {
                    Tabele.Seed();
                    SacuvajPromene();
                }
            }
            else
            {
                Tabele = new TabeleBazaPodataka();
                Tabele.Seed();
                SacuvajPromene(); 
            }
        }
        private bool DaLiJeBazaPrazna()
        {
            return Tabele.Korisnici.Count == 0 &&
                   Tabele.Biljke.Count == 0 &&
                   Tabele.Parfemi.Count == 0 &&
                   Tabele.FiskalniRacuni.Count == 0 &&
                   Tabele.Skladista.Count == 0 &&
                   Tabele.Ambalaze.Count == 0;

        }
        public bool SacuvajPromene()
        {
            try
            {
                string json = JsonConvert.SerializeObject(Tabele, Formatting.Indented);
                File.WriteAllText(putanja, json);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
