using System.Collections.Generic;

namespace PlanSysteem.Models
{
    public sealed class Casemanager : Hulpinstantie
    {
        public List<Gedetineerde> Caseload { get; } = new();
    }
}