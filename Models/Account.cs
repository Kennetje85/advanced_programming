using PlanSysteem.Models;
using System;
using System.Collections.Generic;

namespace PlanSysteem.Models
{
    // FIX: Account remains only in the Models namespace.
    public class Account
    {
        public string Gebruikersnaam { get; }
        public string Wachtwoord { get; }
        public Rol Rol { get; }
        public object Owner { get; }

        public Account(string user, string pass, Rol rol, object owner)
        {
            Gebruikersnaam = user ?? "";
            Wachtwoord = pass ?? "";
            Rol = rol;
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        // Domain logic only: verify password (no IO).
        public bool VerifyPassword(string password) => Wachtwoord == password;

        // Kept for compatibility; prefer using an IAuthenticator implementation.
        [Obsolete("Use VerifyPassword and an IAuthenticator strategy for authentication.")]
        public bool Inloggen(string u, string p)
            => string.Equals(Gebruikersnaam, u, StringComparison.OrdinalIgnoreCase) && VerifyPassword(p);
    }
}

namespace PlanSysteem.Services
{
    // Strategy pattern interface for authentication.
    // Placed in Services namespace and file so it can reference PlanSysteem.Models.Account cleanly.
    public interface IAuthenticator
    {
        Account? Authenticate(IEnumerable<Account> accounts, string username, string password);
    }
}