using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using PlanSysteem.Factories;
using PlanSysteem.Models;

namespace PlanSysteem
{
    // Opmerking: dit bestand heet 'IAccountFactory.cs' maar bevat hier een 'Program' klasse.
    // Overweeg het bestand te hernoemen naar 'Program.cs' voor duidelijkheid en consistentie.
    public class Program
    {
        public static void Main()
        {
            // Lees configuratie/definities van hulpinstanties uit een JSON-bestand.
            // Aangepast: gebruik van een DTO ('HulpinstantieDto') en Factory ('HulpinstantieFactory')
            // om object-creatie te centraliseren i.p.v. overal 'new' te gebruiken.
            var json = File.ReadAllText("hulpinstanties.json");
            var dtos = JsonSerializer.Deserialize<List<HulpinstantieDto>>(json) ?? new List<HulpinstantieDto>();

            // Factory creëert concrete subtypes van Hulpinstantie op basis van de DTO.
            var hulpFactory = new HulpinstantieFactory();
            var hulpinstanties = hulpFactory.CreateMany(dtos).ToList();

            // Voorbeeld: maak accounts via een aparte AccountFactory.
            // Aangepast: centraliseert account-initialisatie (rol-vertaling, validatie, owner-koppeling).
            var accountFactory = new AccountFactory();
            foreach (var h in hulpinstanties)
            {
                // Bepaal rolnaam op basis van het concrete type.
                // Let op: zorg dat de namen hier overeenkomen met de waarden in de enum `Rol`.
                var rolNaam = h.GetType().Name switch
                {
                    "MedischeDienst" => "MedischeDienst",
                    "GeestelijkVerzorger" => "GeestelijkVerzorger",
                    "Casemanager" => "Casemanager",
                    "Afdelingshoofd" => "Afdelingshoofd",
                    _ => "MedischeDienst"
                };

                // Aangepast: maak account via factory. Hier gebruiken we tijdelijk een standaardwachtwoord.
                // Productie: vervang dit door veilige wachtwoord-generatie of een secrets-oplossing.
                h.Account = accountFactory.Create(h.Naam.ToLower(), "wachtwoord123", rolNaam, h);
            }

            // Einde bootstrap. Vanaf hier kan de rest van de applicatie verdergaan met de aangemaakte objecten.
            // Tip: registreer de factories in een DI-container (__IServiceCollection.AddSingleton__ / __AddScoped__)
            // zodat ze elders via dependency injection gebruikt kunnen worden.
        }
    }
}