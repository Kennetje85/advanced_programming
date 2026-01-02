using System;
using System.Collections.Generic;
using System.Linq;
using Hanssens.Net;
using PlanSysteem.Services;

namespace PlanSysteem.Models
{
    // Hulpinstantie is nu een Account-eigenaar (implementeert IAccountOwner).
    public abstract class Hulpinstantie : IAccountOwner
    {
        public int Id { get; set; }
        public string Naam { get; set; } = "";
        public Agenda Agenda { get; } = new();
        public Account Account { get; set; } = default!;

        // Robuuste interactieve flow: blijft in een loop totdat gebruiker terugkeert.
        public void RunFlowHulpinstantie()
        {
            try
            {
                using var localStorage = new LocalStorage();
                localStorage.Load();

                while (true)
                {
                    Console.WriteLine();
                    Console.WriteLine("1. Afspraken inzien");
                    Console.WriteLine("2. Beschikbaarheid opgeven");
                    Console.WriteLine("0. Terug / Uitloggen");
                    Console.Write("Keuze: ");
                    var input = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(input))
                    {
                        Console.WriteLine("Ongeldige invoer.");
                        continue;
                    }

                    if (!int.TryParse(input, out var k))
                    {
                        Console.WriteLine("Ongeldige invoer, voer een nummer in.");
                        continue;
                    }

                    if (k == 0)
                    {
                        Console.WriteLine("Terug naar hoofdmenu...");
                        return;
                    }

                    switch (k)
                    {
                        case 1:
                        {
                            // (her)laad de storage direct voordat we uitlezen
                            localStorage.Load();

                            var key = $"Afspraken:hulp:{Id}";
                            var afspraken = localStorage.Exists(key)
                                ? localStorage.Get<List<string>>(key) ?? new List<string>()
                                : new List<string>();

                            if (afspraken.Count == 0)
                            {
                                Console.WriteLine($"{Naam} heeft nog geen afspraken.");
                            }
                            else
                            {
                                Console.WriteLine($"Afspraken voor {Naam} (totaal: {afspraken.Count}):");
                                for (int i = 0; i < afspraken.Count; i++)
                                    Console.WriteLine($"{i + 1}. {afspraken[i]}");
                            }
                            break;
                        }

                        case 2:
                        {
                            Console.WriteLine("Voer in als: YYYY-MM-DD HH:mm,HH:mm");
                            var s = Console.ReadLine();
                            if (TryParse(s, out var datum, out var start, out var eind))
                            {
                                var beschikbaarheid = new Beschikbaarheid
                                {
                                    Id = Guid.NewGuid().GetHashCode(),
                                    Datum = datum,
                                    StartTijd = start,
                                    EindTijd = eind
                                };

                                Agenda.ToevoegenBeschikbaarheid(beschikbaarheid);

                                var key = $"Beschikbaarheid:{Id}";
                                var opgeslagen = localStorage.Exists(key)
                                    ? localStorage.Get<List<Beschikbaarheid>>(key) ?? new List<Beschikbaarheid>()
                                    : new List<Beschikbaarheid>();

                                opgeslagen.Add(beschikbaarheid);

                                localStorage.Store(key, opgeslagen);
                                localStorage.Persist();

                                Console.WriteLine("✅ Beschikbaarheid toegevoegd.");
                                Console.WriteLine($"Totaal beschikbaarheden (voor {Naam}): {opgeslagen.Count}");
                            }
                            else
                            {
                                Console.WriteLine("Onjuiste datum/tijd-indeling.");
                            }

                            break;
                        }

                        default:
                            Console.WriteLine("Ongeldige keuze.");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Er is een fout opgetreden: {ex.Message}");
            }
        }

        protected static bool TryParse(string? s, out DateTime d, out TimeSpan st, out TimeSpan et)
        {
            d = default; st = default; et = default;
            if (string.IsNullOrWhiteSpace(s)) return false;
            var parts = s.Split(' ');
            if (parts.Length != 2) return false;
            if (!DateTime.TryParse(parts[0], out d)) return false;
            var tt = parts[1].Split(',');
            return TimeSpan.TryParse(tt[0], out st) && TimeSpan.TryParse(tt[1], out et);
        }
    }
}