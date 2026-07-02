namespace PlanSysteem.Models
{
    //Interface die aangeeft dat een type eigenaar van een Account kan zijn.
    
    public interface IAccountOwner
    {
        int Id { get; }
        string Naam { get; }
    }
}