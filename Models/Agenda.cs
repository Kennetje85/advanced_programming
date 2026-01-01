namespace PlanSysteem.Models
{
    public class Agenda
    {
        private readonly List<Beschikbaarheid> _beschikbaarheden = new();
        private readonly List<Afspraak> _afspraken = new();

        public IEnumerable<Beschikbaarheid> OphalenBeschikbaarheid(DateTime vanaf) =>
            _beschikbaarheden.Where(b => !b.IsGereserveerd && b.Start >= vanaf).OrderBy(b => b.Start);
        
        public void ToevoegenBeschikbaarheid(Beschikbaarheid b) => _beschikbaarheden.Add(b);

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
        }
    }
}