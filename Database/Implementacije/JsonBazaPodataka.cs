using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.BazaPodataka;
using Newtonsoft.Json;

namespace Database.Implementacije
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
            }
            else
            {
                Tabele = new TabeleBazaPodataka();
                SacuvajPromene(); 
            }
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
