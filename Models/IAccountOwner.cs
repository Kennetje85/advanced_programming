namespace PlanSysteem.Models
{
    // Interface die aangeeft dat een type eigenaar van een Account kan zijn.
    // Houdt alleen domain-eigenschappen (SRP): geen I/O, geen opslag.
    public interface IAccountOwner
    {
        int Id { get; }
        string Naam { get; }
    }
}