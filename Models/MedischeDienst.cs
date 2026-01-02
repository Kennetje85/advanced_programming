namespace PlanSysteem.Models
{
    // Patterns toegepast:
    // - SRP: specialization of Hulpinstantie; geen opslag in dit type.
    public sealed class MedischeDienst : Hulpinstantie
    {
        public MedischeRol MedischeRol { get; set; }
    }

    public enum MedischeRol
    {
        Huisarts,
        Tandarts,
        Fysiotherapeut,
        Verpleegkundige
    }
}



