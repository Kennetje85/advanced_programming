using System;
using System.Collections.Generic;
using PlanSysteem.Models;

namespace PlanSysteem.Factories
{
    // Enkel DTO; implementatie van de factory staat in Factories\HulpinstantieFactory.cs
    public record HulpinstantieDto(int Id, string Type, string Naam, string? Specialisatie, string? Afdeling, string? MedischeRol);
}