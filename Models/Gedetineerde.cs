using Hanssens.Net;

namespace PlanSysteem.Models
{
    public class Gedetineerde
    {
        public int Id { get; set; }
        public string Naam { get; set; } = "";
        public int Celnummer { get; set; }
        public string Afdeling { get; set; } = "";
        public Agenda Agenda { get; } = new();
        public Account Account { get; set; } = default!;

        public void RunFlowGedetineerde(IEnumerable<Hulpinstantie> hulpinstanties)
        {
            Console.WriteLine(
                "1. Afspraken inzien\n2 Afspraak maken");
            if (!int.TryParse(Console.ReadLine(), out var k)) return;

            switch (k)
            {
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

            Console.WriteLine("Kies hulpinstantie (nummer):");
            for (int i = 0; i < lijst.Count; i++)
            {
                var h = lijst[i];
                Console.WriteLine($"{i + 1}. {h.Naam} ({h.GetType().Name})");
            }

            if (!int.TryParse(Console.ReadLine(), out var keuzeH) || keuzeH < 1 || keuzeH > lijst.Count)
            {
                Console.WriteLine("Ongeldige keuze.");
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

            Console.WriteLine("Kies beschikbaarheid (nummer):");
            for (int i = 0; i < opties.Count; i++)
            {
                var b = opties[i];
                Console.WriteLine($"{i + 1}. {b.Datum:yyyy-MM-dd} {b.StartTijd:hh\\:mm}-{b.EindTijd:hh\\:mm}");
            }

            if (!int.TryParse(Console.ReadLine(), out var keuzeB) || keuzeB < 1 || keuzeB > opties.Count)
            {
                Console.WriteLine("Ongeldige keuze.");
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

            Console.WriteLine("✅ Afspraak gemaakt, bevestigd en opgeslagen.");
        }
    }
}
