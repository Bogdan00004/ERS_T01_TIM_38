using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.BazaPodataka;
using Domain.Modeli;
using Services.Interfaces;


namespace Services.Implementacije
{
    public class ProizvodnjaServis:IProizvodnjaServis
    {
        private readonly IBazaPodataka _baza;

        public ProizvodnjaServis(IBazaPodataka baza)
        {
            _baza = baza;
        }
        public void PosadiNovuBiljku(string naziv, string latinskiNaziv, string zemljaPorekla)
        {
            var novaBiljka = new Biljka
            {
                Id = Guid.NewGuid(),
                Naziv = naziv,
                LatinskiNaziv = latinskiNaziv,
                ZemljaPorekla = zemljaPorekla,
                Stanje = StanjeBiljke.Posadjena,
                JacinaAromaticnihUlja = Math.Round(new Random().NextDouble() * 5.0, 2) 
            };

            _baza.Tabele.Biljke.Add(novaBiljka);
            _baza.SacuvajPromene();
        }
        public void PromeniJacinuUlja(Guid idBiljke, double jacinaPreradjeneBiljke)
        {
            var biljka = _baza.Tabele.Biljke.FirstOrDefault(b => b.Id == idBiljke);

            if(biljka == null)
            {
                Console.WriteLine("Greška: Biljka nije pronađena.");
                return;
            }

            if(jacinaPreradjeneBiljke > 4.00)
            {
                double odstupanje = jacinaPreradjeneBiljke - 4.00; // npr: 4.65 → 0.65
                double procenatSmanjenja = odstupanje * 100.0;     // npr: 0.65 * 100 = 65%
                double noviProcenat = 100.0 - procenatSmanjenja;   // 100 - 65 = 35%

                biljka.JacinaAromaticnihUlja = Math.Round(biljka.JacinaAromaticnihUlja * (noviProcenat/100.0), 2);
                _baza.SacuvajPromene();
            }
            else
            {               
                Console.WriteLine("Nema potrebe za smanjenjem jačine – vrednost nije preko 4.00.");
            }
        }
        public List<Biljka> UberiBiljke(string naziv, int kolicina)
        {
            var zaBerbu = _baza.Tabele.Biljke.Where(b => b.Naziv == naziv && b.Stanje == StanjeBiljke.Posadjena).Take(kolicina).ToList();

            foreach(var biljka in zaBerbu)
                biljka.Stanje = StanjeBiljke.Ubrana;

            _baza.SacuvajPromene();
            return zaBerbu;
        }
        public void TestirajSadnjuISmanjenje()
        {
            Console.WriteLine("Sadimo biljku...");
            PosadiNovuBiljku("Lavanda", "Lavandula", "Francuska");

            var nova = _baza.Tabele.Biljke.Last();
            Console.WriteLine($"Pre smanjenja: {nova.JacinaAromaticnihUlja}");

            // Simulacija: preradjena biljka imala 4.65
            PromeniJacinuUlja(nova.Id, 4.65);

            Console.WriteLine($"Posle smanjenja: {nova.JacinaAromaticnihUlja}");
        }
    }
}
