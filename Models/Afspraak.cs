namespace PlanSysteem.Models
{
    public class Afspraak
    {
        public DateTime StartTijd { get; private set; }
        public DateTime EindTijd  { get; private set; }
        public string Omschrijving { get; private set; } = "";
        public Hulpinstantie Hulpinstantie { get; }
        public Gedetineerde Gedetineerde { get; }
        public Beschikbaarheid BronBeschikbaarheid { get; }
        public bool IsBevestigd { get; private set; }
        public string Titel { get; set; } = "";

        public Afspraak(Gedetineerde ged, Hulpinstantie hulp, Beschikbaarheid b, string omschrijving)
        {
            Gedetineerde = ged;
            Hulpinstantie = hulp;
            BronBeschikbaarheid = b;
            StartTijd = b.Start;
            EindTijd = b.Eind;
            Omschrijving = omschrijving ?? "";
            Titel = omschrijving ?? "";
            
        }

        public void Bevestigen() => IsBevestigd = true;

        
        public override string ToString()
            => $"{StartTijd:dd-MM-yyyy HH\\:mm}-{EindTijd:HH\\:mm} | {Omschrijving} {(IsBevestigd ? "(bevestigd)" : "")}";
    }
}