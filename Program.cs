using System;
using System.Collections.Generic;
using System.Linq;
using PlanSysteem.Factories;
using PlanSysteem.Models;
using PlanSysteem.Services;

namespace PlanSysteem
{
    public class Program
    {
        static void Main()
        {
            var factory = new HulpinstantieFactory();



            var hulpinstanties = factory.CreateMany(new[]
            {

                new HulpinstantieDto(1, "MedischeDienst", "Medische Dienst", null, null, "Huisarts"),
                new HulpinstantieDto(2, "GeestelijkVerzorger", "Geestelijk Verzorger", "Pastoor", null, null),
                new HulpinstantieDto(3, "Casemanager", "Casemanager", null, null, null),
                new HulpinstantieDto(4, "Afdelingshoofd", "Afdelingshoofdd", null, "A", null),
          //      new HulpinstantieDto(5, "Psychologie", "Psycholoog", null, "B", null)
            }).ToList();





            var commandManager = new CommandManager();

            // Attach storage subscriber and console logger to each agenda (Observer)
            foreach (var h in hulpinstanties)
            {
                AgendaStorageSubscriber.Attach(h.Agenda);

                var hulp = h;
                hulp.Agenda.BeschikbaarheidToegevoegd += (s, e) =>
                    Console.WriteLine($"[Event] Beschikbaarheid toegevoegd voor {hulp.Naam}: {e.Beschikbaarheid}");
                hulp.Agenda.AfspraakToegevoegd += (s, e) =>
                    Console.WriteLine($"[Event] Afspraak toegevoegd voor {hulp.Naam}: {e.Afspraak}");
                hulp.Agenda.AfspraakVerwijderd += (s, e) =>
                    Console.WriteLine($"[Event] Afspraak verwijderd voor {hulp.Naam}: {e.Afspraak}");
                hulp.Agenda.BeschikbaarheidVrijgemaakt += (s, e) =>
                    Console.WriteLine($"[Event] Beschikbaarheid vrijgemaakt voor {hulp.Naam}: {e.Beschikbaarheid}");
            }

            var md = hulpinstanties.OfType<MedischeDienst>().First();
            md.Agenda.ToevoegenBeschikbaarheid(new Beschikbaarheid
            {
                Id = 101,
                Datum = DateTime.Today.AddDays(1),
                StartTijd = new TimeSpan(9, 0, 0),
                EindTijd = new TimeSpan(9, 30, 0)
            });
            var gv = hulpinstanties.OfType<GeestelijkVerzorger>().First();
            gv.Agenda.ToevoegenBeschikbaarheid(new Beschikbaarheid
            {
                Id = 201,
                Datum = DateTime.Today.AddDays(1),
                StartTijd = new TimeSpan(10, 0, 0),
                EindTijd = new TimeSpan(10, 30, 0)
            });

            var cm = hulpinstanties.OfType<Casemanager>().First();
            var ged = new Gedetineerde { Id = 1, Naam = "Jan", Celnummer = 12, Afdeling = "A" };
            cm.Caseload.Add(ged);
            var ah = hulpinstanties.OfType<Afdelingshoofd>().First();

            // md.Account = new Account("medischedienst", "MD12", Rol.MedischeDienst, md);
            // gv.Account = new Account("geestelijkverzorger", "GV02", Rol.GeestelijkVerzorger, gv);
            // cm.Account = new Account("casemanager", "CM03", Rol.Casemanager, cm);
            // ah.Account = new Account("afdelingshoofd", "AH04", Rol.Afdelingshoofd, ah);
            // ged.Account = new Account("gedetineerde", "GD01", Rol.Gedetineerde, ged);

            // Program.cs blijft netter en makkelijker uitbreidbaar.
            var accountFactory = new AccountFactory();

            md.Account = accountFactory.Create(
                "medischedienst",
                "MD12",
                "MedischeDienst",
                md);

            gv.Account = accountFactory.Create(
                "geestelijkverzorger",
                "GV02",
                "GeestelijkVerzorger",
                gv);

            cm.Account = accountFactory.Create(
                "casemanager",
                "CM03",
                "Casemanager",
                cm);

            ah.Account = accountFactory.Create(
                "afdelingshoofd",
                "AH04",
                "Afdelingshoofd",
                ah);

            ged.Account = accountFactory.Create(
                "gedetineerde",
                "GD01",
                "Gedetineerde",
                ged);



            var accounts = new List<Account> { md.Account, gv.Account, cm.Account, ah.Account, ged.Account };

            Account? acc = null;

            while (acc == null)
            {
                Console.Write("Gebruikersnaam: ");
                var u = Console.ReadLine();

                Console.Write("Wachtwoord: ");
                var p = Console.ReadLine();

                acc = accounts.FirstOrDefault(a => a.Inloggen(u!, p!));

                if (acc == null)
                {
                    Console.WriteLine("Onjuist. Probeer opnieuw.");
                }
            }


            Console.WriteLine($"✅ Ingelogd als {acc.Rol}");
            switch (acc.Owner)
            {
                case Gedetineerde g:
                    Console.WriteLine("Start gedetineerde flow");
                    g.RunFlowGedetineerde(hulpinstanties, commandManager);
                    break;
                case Hulpinstantie h:
                    h.RunFlowHulpinstantie();
                    break;
                default:
                    Console.WriteLine("Onbekende eigenaar van account.");
                    break;
            }
        }
    }
}
