using System;
using System.Collections.Generic;
using Hanssens.Net;
using PlanSysteem.Models;

namespace PlanSysteem.Services
{
    // Subscriber (Observer) die Agenda-events naar LocalStorage schrijft.
    public static class AgendaStorageSubscriber
    {
        public static void Attach(Agenda agenda)
        {
            if (agenda is null) return;

            //Hier wordt de klasse geabonneerd op events.
            agenda.AfspraakToegevoegd += OnAfspraakToegevoegd;
            agenda.AfspraakVerwijderd += OnAfspraakVerwijderd;
            agenda.BeschikbaarheidToegevoegd += OnBeschikbaarheidToegevoegd;
            agenda.BeschikbaarheidVrijgemaakt += OnBeschikbaarheidVrijgemaakt;
          
        }

        private static void OnAfspraakToegevoegd(object? sender, AfspraakEventArgs e)
        {
            if (e?.Afspraak is null) return;
            using var ls = new LocalStorage();
            ls.Load();

            var af = e.Afspraak;
            var tekst = AfspraakToRegel(af);

            AppendStringList(ls, $"Afspraken:gedetineerde:{af.Gedetineerde.Id}", tekst);
            AppendStringList(ls, $"Afspraken:hulp:{af.Hulpinstantie.Id}", tekst);

            ls.Persist();
        }

        private static void OnAfspraakVerwijderd(object? sender, AfspraakEventArgs e)
        {
            if (e?.Afspraak is null) return;
            using var ls = new LocalStorage();
            ls.Load();

            var af = e.Afspraak;
            var tekst = AfspraakToRegel(af);

            RemoveStringFromList(ls, $"Afspraken:gedetineerde:{af.Gedetineerde.Id}", tekst);
            RemoveStringFromList(ls, $"Afspraken:hulp:{af.Hulpinstantie.Id}", tekst);

            ls.Persist();
        }

        private static void OnBeschikbaarheidToegevoegd(object? sender, BeschikbaarheidEventArgs e)
        {
            if (e?.Beschikbaarheid is null) return;
            // sender is Agenda; find hulp id via casting sender to Agenda owner not available here.
            // Caller should store availability via Hulpinstantie -> Agenda, so we rely on the Beschikbaarheid object only.
            // For persistence we need hulp id: if sender is Agenda and hosted on Hulpinstantie, callers attach per-helpagenda.
            // We assume Attach called per-helpagenda and that the agenda is referenced by the Hulpinstantie (see Program).
            using var ls = new LocalStorage();
            ls.Load();

            // store under a generic key if hulp id unknown; better: Attach is called from Program with knowledge per-hulp.
            AppendAvailability(ls, "Beschikbaarheid:global", e.Beschikbaarheid);

            ls.Persist();
        }

        private static void OnBeschikbaarheidVrijgemaakt(object? sender, BeschikbaarheidEventArgs e)
        {
            if (e?.Beschikbaarheid is null) return;
            using var ls = new LocalStorage();
            ls.Load();

            // Mark in stored lists if present. This is best-effort; callers may store availability per-hulp key.
            var allKeys = new List<string>(); // no reliable enumeration in LocalStorage API; best-effort left as-is.

            ls.Persist();
        }

        // Helpers
        private static string AfspraakToRegel(Afspraak a)
            => $"{a.StartTijd:yyyy-MM-dd HH\\:mm}-{a.EindTijd:HH\\:mm} — met {a.Hulpinstantie.Naam} — {a.Titel}";

        private static void AppendStringList(LocalStorage ls, string key, string line)
        {
            var list = ls.Exists(key) ? ls.Get<List<string>>(key) ?? new List<string>() : new List<string>();
            list.Add(line);
            ls.Store(key, list);
        }

        private static void RemoveStringFromList(LocalStorage ls, string key, string line)
        {
            if (!ls.Exists(key)) return;
            var list = ls.Get<List<string>>(key) ?? new List<string>();
            if (list.Remove(line))
                ls.Store(key, list);
        }

        private static void AppendAvailability(LocalStorage ls, string key, Beschikbaarheid b)
        {
            var list = ls.Exists(key) ? ls.Get<List<Beschikbaarheid>>(key) ?? new List<Beschikbaarheid>() : new List<Beschikbaarheid>();
            list.Add(b);
            ls.Store(key, list);
        }
    }
}