using System;
using System.Collections.Generic;
using System.Linq;

namespace PlanSysteem.Models
{
    public sealed class AfspraakEventArgs : EventArgs
    {
        public Afspraak Afspraak { get; }
        public AfspraakEventArgs(Afspraak afspraak) => Afspraak = afspraak;
    }

    public sealed class BeschikbaarheidEventArgs : EventArgs
    {
        public Beschikbaarheid Beschikbaarheid { get; }
        public BeschikbaarheidEventArgs(Beschikbaarheid b) => Beschikbaarheid = b;
    }
    //List<T>
    public class Agenda
    {
        private readonly List<Beschikbaarheid> _beschikbaarheden = new();
        private readonly List<Afspraak> _afspraken = new();

        // Observer events
        public event EventHandler<BeschikbaarheidEventArgs>? BeschikbaarheidToegevoegd;
        public event EventHandler<BeschikbaarheidEventArgs>? BeschikbaarheidVrijgemaakt;
        public event EventHandler<AfspraakEventArgs>? AfspraakToegevoegd;
        public event EventHandler<AfspraakEventArgs>? AfspraakVerwijderd;

        public IEnumerable<Beschikbaarheid> OphalenBeschikbaarheid(DateTime vanaf) =>
            _beschikbaarheden.Where(b => !b.IsGereserveerd && b.Start >= vanaf).OrderBy(b => b.Start);


        // Big-o notation for the methods: 0(1)
        public void ToevoegenBeschikbaarheid(Beschikbaarheid b)
        {
            _beschikbaarheden.Add(b);
            OnBeschikbaarheidToegevoegd(b);
        }

        public Beschikbaarheid? Reserveer(int id)
        {
            var b = _beschikbaarheden.FirstOrDefault(x => x.Id == id && !x.IsGereserveerd);
            if (b is null) return null;
            b.IsGereserveerd = true;
            return b;
        }

        public void VoegAfspraakToe(Afspraak a)
        {
            _afspraken.Add(a);
            OnAfspraakToegevoegd(a);
        }

        public void VerwijderAfspraak(Afspraak a)
        {
            if (a == null) return;
            if (_afspraken.Remove(a))
                OnAfspraakVerwijderd(a);
        }

        public void MaakBeschikbaarheidVrij(int id)
        {
            var b = _beschikbaarheden.FirstOrDefault(x => x.Id == id);
            if (b != null)
            {
                b.IsGereserveerd = false;
                OnBeschikbaarheidVrijgemaakt(b);
            }
        }

        public IEnumerable<Afspraak> OphalenAfspraakSnapshot() => _afspraken.ToList();

        protected virtual void OnBeschikbaarheidToegevoegd(Beschikbaarheid b)
            => BeschikbaarheidToegevoegd?.Invoke(this, new BeschikbaarheidEventArgs(b));

        protected virtual void OnBeschikbaarheidVrijgemaakt(Beschikbaarheid b)
            => BeschikbaarheidVrijgemaakt?.Invoke(this, new BeschikbaarheidEventArgs(b));

        protected virtual void OnAfspraakToegevoegd(Afspraak a)
            => AfspraakToegevoegd?.Invoke(this, new AfspraakEventArgs(a));

        protected virtual void OnAfspraakVerwijderd(Afspraak a)
            => AfspraakVerwijderd?.Invoke(this, new AfspraakEventArgs(a));
    }
}