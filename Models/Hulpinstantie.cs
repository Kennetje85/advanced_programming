using Hanssens.Net;
using System.Collections.Generic;
using PlanSysteem.Services;

namespace PlanSysteem.Models
{
    public abstract class Hulpinstantie
    {
        public int Id { get; set; }
        public string Naam { get; set; } = "";
        public Agenda Agenda { get; } = new();
        public Account Account { get; set; } = default!;
        
        

        public void RunFlowHulpinstantie()
        {
            using var localStorage = new LocalStorage();
            localStorage.Load();

            Console.WriteLine("1. Afspraken inzien.\n2. Beschikbaarheid opgeven");
            Console.WriteLine("Antwoord doormiddel van een cijfer");
            if (!int.TryParse(Console.ReadLine(), out var k)) return;

            switch (k)
            {
                case 1:
                {
                    using var ls = new LocalStorage();
                    ls.Load();

                    var key = $"Afspraken:hulp:{Id}";
                    if (!ls.Exists(key))
                    {
                        Console.WriteLine($"{Naam} heeft nog geen afspraken.");
                        break;
                    }

                    var afspraken = ls.Get<List<string>>(key);
                    if (afspraken == null || afspraken.Count == 0)
                    {
                        Console.WriteLine($"{Naam} heeft nog geen afspraken.");
                        break;
                    }

                    Console.WriteLine($"Afspraken voor {Naam}:");
                    for (int i = 0; i < afspraken.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {afspraken[i]}");
                    }

                    break;
                }


                case 2:
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
                    break;
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