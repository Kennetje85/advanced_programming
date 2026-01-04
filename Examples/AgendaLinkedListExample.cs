using System;
using System.Collections.Generic;
using PlanSysteem.Models;

namespace PlanSysteem.Examples
{
    // Voorbeeld: vervang _afspraken List door LinkedList voor efficiënte verwijdering tijdens iteratie.
    public static class AgendaLinkedListExample
    {
        public static void Demo(Agenda agenda, Afspraak toRemove)
        {
            // Convert (of houd LinkedList in Agenda)
            var linked = new LinkedList<Afspraak>(agenda.OphalenAfspraakSnapshot());

            // Itereren en verwijderen veilig tijdens iteratie
            var node = linked.First;
            while (node != null)
            {
                var next = node.Next;
                if (node.Value == toRemove)
                    linked.Remove(node); // O(1) verwijdering
                node = next;
            }

            // Eventueel terugzetten naar agenda's interne opslag of gebruiken zoals gewenst.
            Console.WriteLine("LinkedList demo klaar.");
        }
    }
}