namespace PlanSysteem.Repositories
{
    // Alleen wat nodig is door Gedetineerde/Hulpinstantie voor afspraak-persistentie.
    public interface IAfsprakenRepository
    {
        List<string> GetAfsprakenVoorGedetineerde(int gedId);
        List<string> GetAfsprakenVoorHulp(int hulpId);
        void AddAfsprakenVoorGedetineerde(int gedId, string regel);
        void AddAfsprakenVoorHulp(int hulpId, string regel);
    }
}