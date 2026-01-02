using PlanSysteem.Models;
using PlanSysteem.Commands;

namespace PlanSysteem.Commands
{
    // Eenvoudig voorbeeld: voert maken van afspraak uit en kan dit ongedaan maken.
    public class CreateAfspraakCommand : ICommand
    {
        private readonly Gedetineerde _ged;
        private readonly Hulpinstantie _hulp;
        private readonly Beschikbaarheid _beschikbaarheid;
        private Afspraak? _afspraak;

        public CreateAfspraakCommand(Gedetineerde ged, Hulpinstantie hulp, Beschikbaarheid b)
        {
            _ged = ged;
            _hulp = hulp;
            _beschikbaarheid = b;
        }

        public void Execute()
        {
            // Reserveer slot
            _beschikbaarheid.IsGereserveerd = true;

            // Maak afspraak en bevestig
            _afspraak = new Afspraak(_ged, _hulp, _beschikbaarheid, $"Afspraak met {_hulp.Naam}");
            _afspraak.Bevestigen();

            // Voeg toe aan agenda's
            _hulp.Agenda.VoegAfspraakToe(_afspraak);
            _ged.Agenda.VoegAfspraakToe(_afspraak);
        }

        public void Undo()
        {
            if (_afspraak is null) return;

            // Verwijder afspraak uit agenda's
            _hulp.Agenda.VerwijderAfspraak(_afspraak);
            _ged.Agenda.VerwijderAfspraak(_afspraak);

            // Maak beschikbaarheid weer vrij
            _beschikbaarheid.IsGereserveerd = false;
        }
    }
}