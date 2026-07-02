using System;
using System.Collections.Generic;
using System.Linq;
using Hanssens.Net;
using PlanSysteem.Commands;
using PlanSysteem.Services;

namespace PlanSysteem.Models
{
    public class Gedetineerde : IAccountOwner
    {
        public int Id { get; set; }
        public string Naam { get; set; } = "";
        public int Celnummer { get; set; }
        public string Afdeling { get; set; } = "";
        public Agenda Agenda { get; } = new();
        public Account Account { get; set; } = default!;

        // Command pattern:
        // - Afspraak maken gebeurt via CommandManager.
        // - Undo/redo is nu ook echt beschikbaar in de flow.
        public void RunFlowGedetineerde(IEnumerable<Hulpinstantie> hulpinstanties, CommandManager commandManager)
        {
            if (commandManager is null) throw new ArgumentNullException(nameof(commandManager));

            while (true)
            {
                Console.WriteLine("1. Afspraken inzien\n2. Afspraak maken\n3. Undo\n4. Redo\n0. Uitloggen");
                Console.Write("Keuze: ");
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input)) continue;

                if (!int.TryParse(input, out var k))
                {
                    Console.WriteLine("Ongeldige invoer.");
                    continue;
                }

                switch (k)
                {
                    case 0:
                        Console.WriteLine("Uitloggen...");
                        return;

                    case 1:
                    {
                        using var ls = new LocalStorage();
                        ls.Load();

                        var key = $"Afspraken:gedetineerde:{Id}";
                        var regels = ls.Exists(key)
                            ? ls.Get<List<string>>(key) ?? new List<string>()
                            : new List<string>();

                        if (regels.Count == 0)
                        {
                            Console.WriteLine($"{Naam} heeft geen afspraken.");
                        }
                        else
                        {
                            Console.WriteLine($"Afspraken voor {Naam}:");
                            for (int i = 0; i < regels.Count; i++)
                            {
                                Console.WriteLine($"{i + 1}. {regels[i]}");
                            }
                        }

                        break;
                    }

                    case 2:
                        MaakAfspraak(hulpinstanties, commandManager);
                        break;

                    case 3:
                        if (commandManager.CanUndo)
                        {
                            commandManager.Undo();
                            Console.WriteLine("✅ Laatste actie ongedaan gemaakt.");
                        }
                        else
                        {
                            Console.WriteLine("Niets om ongedaan te maken.");
                        }
                        break;

                    case 4:
                        if (commandManager.CanRedo)
                        {
                            commandManager.Redo();
                            Console.WriteLine("✅ Laatste actie opnieuw uitgevoerd.");
                        }
                        else
                        {
                            Console.WriteLine("Niets om opnieuw uit te voeren.");
                        }
                        break;

                    default:
                        Console.WriteLine("Ongeldige keuze.");
                        break;
                }
            }
        }

        private void MaakAfspraak(IEnumerable<Hulpinstantie> hulpinstanties, CommandManager commandManager)
        {
            var lijst = hulpinstanties.ToList();
            if (!lijst.Any())
            {
                Console.WriteLine("Er zijn geen hulpinstanties beschikbaar.");
                return;
            }

            while (true)
            {
                Console.WriteLine("Kies hulpinstantie (nummer) of 0 om te annuleren:");
                for (int i = 0; i < lijst.Count; i++)
                {
                    var h = lijst[i];
                    Console.WriteLine($"{i + 1}. {h.Naam} ({h.GetType().Name})");
                }

                var keuzeHInput = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(keuzeHInput))
                {
                    Console.WriteLine("Ongeldige invoer.");
                    continue;
                }

                if (!int.TryParse(keuzeHInput, out var keuzeH) || keuzeH < 0 || keuzeH > lijst.Count)
                {
                    Console.WriteLine("Ongeldige keuze.");
                    continue;
                }

                if (keuzeH == 0)
                {
                    Console.WriteLine("Afspraak annuleren.");
                    return;
                }

                var hulp = lijst[keuzeH - 1];

                using var ls = new LocalStorage();
                ls.Load();

                var key = $"Beschikbaarheid:{hulp.Id}";
                var opgeslagenVoorDeze = ls.Exists(key)
                    ? ls.Get<List<Beschikbaarheid>>(key) ?? new List<Beschikbaarheid>()
                    : new List<Beschikbaarheid>();

                if (!opgeslagenVoorDeze.Any())
                {
                    Console.WriteLine($"Nog geen opgeslagen beschikbaarheden voor {hulp.Naam}.");
                    return;
                }

                foreach (var b in opgeslagenVoorDeze)
                    hulp.Agenda.ToevoegenBeschikbaarheid(b);

                var opties = hulp.Agenda.OphalenBeschikbaarheid(DateTime.Now).ToList();
                if (!opties.Any())
                {
                    Console.WriteLine("Geen vrije tijden.");
                    return;
                }

                while (true)
                {
                    Console.WriteLine("Kies beschikbaarheid (nummer) of 0 om te annuleren:");
                    for (int i = 0; i < opties.Count; i++)
                    {
                        var b = opties[i];
                        Console.WriteLine($"{i + 1}. {b.Datum:yyyy-MM-dd} {b.StartTijd:hh\\:mm}-{b.EindTijd:hh\\:mm}");
                    }

                    var keuzeBInput = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(keuzeBInput))
                    {
                        Console.WriteLine("Ongeldige invoer.");
                        continue;
                    }

                    if (!int.TryParse(keuzeBInput, out var keuzeB) || keuzeB < 0 || keuzeB > opties.Count)
                    {
                        Console.WriteLine("Ongeldige keuze.");
                        continue;
                    }

                    if (keuzeB == 0)
                    {
                        Console.WriteLine("Afspraak annuleren.");
                        return;
                    }

                    var gekozenSlot = opties[keuzeB - 1];
                    var command = new CreateAfspraakCommand(this, hulp, gekozenSlot);

                    commandManager.ExecuteCommand(command);

                    Console.WriteLine("✅ Afspraak gemaakt, bevestigd en opgeslagen.");
                    return;
                }
            }
        }
    }
}
