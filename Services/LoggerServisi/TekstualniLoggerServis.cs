using Domain.Enumeracije;
using Domain.Servisi;

namespace Services.LoggerServisi
{
    public class TekstualniLoggerServis : ILoggerServis
    {
        private readonly string _putanja;
        private static readonly object _lockObj = new object();
        public TekstualniLoggerServis(string putanja = "log.txt")
        {
            _putanja = putanja;
        }

        public bool LogInfo(string poruka)
        {
            return EvidentirajDogadjaj(TipEvidencije.INFO, poruka);
        }

        public bool LogWarning(string poruka)
        {
            return EvidentirajDogadjaj(TipEvidencije.WARNING, poruka);
        }

        public bool LogError(string poruka)
        {
            return EvidentirajDogadjaj(TipEvidencije.ERROR, poruka);
        }

        public bool EvidentirajDogadjaj(TipEvidencije tip, string poruka)
        {
            try
            {
                string ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string msg = poruka ?? string.Empty;

                string linija = $"{tip} | {ts} | {msg}";

                lock (_lockObj)
                {
                    File.AppendAllText(_putanja, linija + Environment.NewLine);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
