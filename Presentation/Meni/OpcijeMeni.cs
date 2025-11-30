using Domain.Servisi;
namespace Presentation.Meni
{
    public class OpcijeMeni
    {
        private readonly ISkladistenjeServis _skladistenjeServis;

        public OpcijeMeni(ISkladistenjeServis skladistenjeServis)
        {
            _skladistenjeServis = skladistenjeServis;
        }

        public void PrikaziMeni()
        {
            Console.WriteLine("\n============================================ Meni ===========================================");

            bool kraj = false;
            while (!kraj)
            {
                // TODO: Prikaz opcija menija
            }
        }
    }

}
