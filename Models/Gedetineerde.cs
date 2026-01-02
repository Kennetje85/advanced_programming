using Hanssens.Net;
using System;
using System.Collections.Generic;
using System.Linq;

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

        // Hersteld: originele, robuuste console-flow met LocalStorage.
        public void RunFlowGedetineerde(IEnumerable<Hulpinstantie> hulpinstanties)
        {
            while (true)
            {
                Console.WriteLine("1. Afspraken inzien\n2. Afspraak maken\n0. Uitloggen");
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
                        MaakAfspraak(hulpinstanties);
                        break;

                    default:
                        Console.WriteLine("Ongeldige keuze.");
                        break;
                }
            }
        }

        private void MaakAfspraak(IEnumerable<Hulpinstantie> hulpinstanties)
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
                if (string.IsNullOrWhiteSpace(keuzeHInput)) { Console.WriteLine("Ongeldige invoer."); continue; }
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
                    if (string.IsNullOrWhiteSpace(keuzeBInput)) { Console.WriteLine("Ongeldige invoer."); continue; }
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
                    var gereserveerd = hulp.Agenda.Reserveer(gekozenSlot.Id);
                    if (gereserveerd is null)
                    {
                        Console.WriteLine("Niet beschikbaar (mogelijk net gereserveerd).");
                        return;
                    }

                    var afspraak = new Afspraak(this, hulp, gereserveerd, $"Afspraak met {hulp.Naam}");
                    afspraak.Bevestigen();

                    hulp.Agenda.VoegAfspraakToe(afspraak);
                    Agenda.VoegAfspraakToe(afspraak);

                    string regelVoorGedetineerde =
                        $"{gekozenSlot.Datum:yyyy-MM-dd} {gekozenSlot.StartTijd:hh\\:mm}-{gekozenSlot.EindTijd:hh\\:mm} — met {hulp.Naam} — {afspraak.Titel}";
                    string regelVoorHulp =
                        $"{gekozenSlot.Datum:yyyy-MM-dd} {gekozenSlot.StartTijd:hh\\:mm}-{gekozenSlot.EindTijd:hh\\:mm} — met {Naam} — {afspraak.Titel}";

                    void Append(string k, string line)
                    {
                        var list = ls.Exists(k)
                            ? ls.Get<List<string>>(k) ?? new List<string>()
                            : new List<string>();
                        list.Add(line);
                        ls.Store(k, list);
                    }

                    Append($"Afspraken:gedetineerde:{Id}", regelVoorGedetineerde);
                    Append($"Afspraken:hulp:{hulp.Id}", regelVoorHulp);
                    ls.Persist();

                    // Debug: toon wat er nu in storage staat voor beide keys
                    var storedGed = ls.Get<List<string>>($"Afspraken:gedetineerde:{Id}") ?? new List<string>();
                    var storedHulp = ls.Get<List<string>>($"Afspraken:hulp:{hulp.Id}") ?? new List<string>();
                    Console.WriteLine($"(Debug) Afspraken opgeslagen voor gedetineerde {Id}: {storedGed.Count}");
                    Console.WriteLine($"(Debug) Afspraken opgeslagen voor hulp {hulp.Id}: {storedHulp.Count}");

                    Console.WriteLine("✅ Afspraak gemaakt, bevestigd en opgeslagen.");
                    return;
                }
            }
        }
    }
}
