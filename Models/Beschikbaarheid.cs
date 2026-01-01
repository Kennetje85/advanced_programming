using System;

namespace PlanSysteem.Models
{
    public class Beschikbaarheid
    {
        public int Id { get; set; }
        public DateTime Datum { get; set; }
        public TimeSpan StartTijd { get; set; }
        public TimeSpan EindTijd  { get; set; }
        public bool IsGereserveerd { get; set; }

        public DateTime Start => Datum.Date + StartTijd;
        public DateTime Eind  => Datum.Date + EindTijd;

        public override string ToString()
            => $"[{Id}] {Start:dd-MM-yyyy HH\\:mm} - {Eind:HH\\:mm}" + (IsGereserveerd ? " (bezet)" : "");
    }
}