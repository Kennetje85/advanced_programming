csharp Factories\HulpinstantieFactory.cs
using System;
using System.Collections.Generic;
using PlanSysteem.Models;

namespace PlanSysteem.Factories
{
    public record HulpinstantieDto(int Id, string Type, string Naam, string? Specialisatie, string? Afdeling, string? MedischeRol);

    public interface IHulpinstantieFactory
    {
        Hulpinstantie Create(HulpinstantieDto dto);
        IEnumerable<Hulpinstantie> CreateMany(IEnumerable<HulpinstantieDto> dtos);
    }

    public class HulpinstantieFactory : IHulpinstantieFactory
    {
        public Hulpinstantie Create(HulpinstantieDto dto)
        {
            return dto.Type switch
            {
                "MedischeDienst" => new MedischeDienst
                {
                    Id = dto.Id,
                    Naam = dto.Naam,
                    MedischeRol = Enum.TryParse<MedischeRol>(dto.MedischeRol ?? "", true, out var mr) ? mr : MedischeRol.Huisarts
                },
                "GeestelijkVerzorger" => new GeestelijkVerzorger
                {
                    Id = dto.Id,
                    Naam = dto.Naam,
                    Specialisatie = dto.Specialisatie
                },
                "Casemanager" => new Casemanager
                {
                    Id = dto.Id,
                    Naam = dto.Naam
                },
                "Afdelingshoofd" => new Afdelingshoofd
                {
                    Id = dto.Id,
                    Naam = dto.Naam,
                    afdeling = dto.Afdeling
                },
                _ => throw new ArgumentException($"Onbekend type: {dto.Type}", nameof(dto))
            };
        }

        public IEnumerable<Hulpinstantie> CreateMany(IEnumerable<HulpinstantieDto> dtos)
        {
            foreach (var d in dtos)
                yield return Create(d);
        }
    }
}