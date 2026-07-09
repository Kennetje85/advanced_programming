using System;
using System.Collections.Generic;
using PlanSysteem.Models;

namespace PlanSysteem.Factories
{

    //Geef mij een HulpinstantieDto en ik geef een Hulpinstantie terug
    public interface IHulpinstantieFactory
    {
        Hulpinstantie Create(HulpinstantieDto dto);

        //Lijst met hulpinstanties maken van een lijst met HulpinstantieDto's
        IEnumerable<Hulpinstantie> CreateMany(IEnumerable<HulpinstantieDto> dtos);
    }

    public class HulpinstantieFactory : IHulpinstantieFactory
    {
        public Hulpinstantie Create(HulpinstantieDto dto)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto));

            var md = new MedischeDienst
            {
                Id = dto.Id,
                Naam = dto.Naam,
                MedischeRol = Enum.TryParse<MedischeRol>(dto.MedischeRol ?? "", true, out var mr) ? mr : MedischeRol.Huisarts
            };
            var gv = new GeestelijkVerzorger
            {
                Id = dto.Id,
                Naam = dto.Naam,
                Specialisatie = dto.Specialisatie
            };
            var cm = new Casemanager
            {
                Id = dto.Id,
                Naam = dto.Naam
            };
            var ah = new Afdelingshoofd
            {
                Id = dto.Id,
                Naam = dto.Naam,
                afdeling = dto.Afdeling
            };


        

            return dto.Type switch
            {
                "MedischeDienst" => md,
                "GeestelijkVerzorger" => gv,
                "Casemanager" => cm,
                "Afdelingshoofd" => ah,
             //   "Psychologie" => ps,
                _ => throw new ArgumentException($"Onbekend type: {dto.Type}", nameof(dto))
            };
        }

        public IEnumerable<Hulpinstantie> CreateMany(IEnumerable<HulpinstantieDto> dtos)
        {
            if (dtos is null) yield break;
            foreach (var d in dtos)
                yield return Create(d);
        }
    }
}