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
            var commandManager = new CommandManager();

            // 1) Domeinobjecten
            var md = new MedischeDienst { Id = 1, Naam = "Medische Dienst", MedischeRol = MedischeRol.Huisarts };
            var gv = new GeestelijkVerzorger { Id = 2, Naam = "Geestelijk Verzorger", Specialisatie = "Pastoor" };
            var cm = new Casemanager { Id = 3, Naam = "Casemanager" };
            var ah = new Afdelingshoofd { Id = 4, Naam = "Afdelingshoofd" };
            var hulpinstanties = new List<Hulpinstantie> { md, gv, cm, ah };

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

            md.Agenda.ToevoegenBeschikbaarheid(new Beschikbaarheid
            {
                Id = 101,
                Datum = DateTime.Today.AddDays(1),
                StartTijd = new TimeSpan(9, 0, 0),
                EindTijd = new TimeSpan(9, 30, 0)
            });
            gv.Agenda.ToevoegenBeschikbaarheid(new Beschikbaarheid
            {
                Id = 201,
                Datum = DateTime.Today.AddDays(1),
                StartTijd = new TimeSpan(10, 0, 0),
                EindTijd = new TimeSpan(10, 30, 0)
            });

            var ged = new Gedetineerde { Id = 1, Naam = "Jan", Celnummer = 12, Afdeling = "A" };
            cm.Caseload.Add(ged);

            md.Account = new Account("medischedienst", "MD12", Rol.MedischeDienst, md);
            gv.Account = new Account("geestelijkverzorger", "GV02", Rol.GeestelijkVerzorger, gv);
            cm.Account = new Account("casemanager", "CM03", Rol.Casemanager, cm);
            ah.Account = new Account("afdelingshoofd", "AH04", Rol.Afdelingshoofd, ah);
            ged.Account = new Account("gedetineerde", "GD01", Rol.Gedetineerde, ged);

            var accounts = new List<Account> { md.Account, gv.Account, cm.Account, ah.Account, ged.Account };

            Console.Write("Gebruikersnaam: ");
            var u = Console.ReadLine();
            Console.Write("Wachtwoord: ");
            var p = Console.ReadLine();
            var acc = accounts.FirstOrDefault(a => a.Inloggen(u!, p!));
            if (acc is null)
            {
                Console.WriteLine("Onjuist.");
                return;
            }

            Console.WriteLine($"✅ Ingelogd als {acc.Rol}");
            switch (acc.Owner)
            {
                case Gedetineerde g:
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
