using Domain.Enumeracije;

namespace Domain.Servisi
{
    public interface ILoggerServis
    {
        bool LogInfo(string poruka);
        bool LogWarning(string poruka);
        bool LogError(string poruka);
        public bool EvidentirajDogadjaj(TipEvidencije tip, string poruka);
    }
}
