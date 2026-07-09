using System;
using PlanSysteem.Models;

namespace PlanSysteem.Models
{
    public class Account
    {
        public string Gebruikersnaam { get; }
        public string Wachtwoord { get; }
        public Rol Rol { get; }
        public IAccountOwner Owner { get; }

        public Account(string user, string pass, Rol rol, IAccountOwner owner)
        {
            Gebruikersnaam = user ?? "";
            Wachtwoord = pass ?? "";
            Rol = rol;
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            //Onoverzichtelijk : de eigenaar kan van verschillende typen zijn, afhankelijk van de rol. Daarom gebruiken we IAccountOwner als type.
            // public Gedetineerde Gedetineerde { get; }
            //  public MedischeDienst MedischeDienst { get; }
            //  public Casemanager Casemanager { get; }


        }

        public bool VerifyPassword(string password) => Wachtwoord == password;

        [Obsolete("Use VerifyPassword and an IAuthenticator strategy for authentication.")]
        public bool Inloggen(string u, string p)
            => string.Equals(Gebruikersnaam, u, StringComparison.OrdinalIgnoreCase) && VerifyPassword(p);
    }
}

namespace PlanSysteem.Services
{
    public interface IAuthenticator
    {
        Account? Authenticate(IEnumerable<Account> accounts, string username, string password);
    }
}