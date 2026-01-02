using System;
using PlanSysteem.Models;

namespace PlanSysteem.Factories
{
    // Interface en factory voor Account-creatie.
    // Aangepast: vierde parameter is nu IAccountOwner (geen object meer).
    public interface IAccountFactory
    {
        Account Create(string gebruikersnaam, string wachtwoord, string rolNaam, IAccountOwner owner);
    }

    public class AccountFactory : IAccountFactory
    {
        public Account Create(string gebruikersnaam, string wachtwoord, string rolNaam, IAccountOwner owner)
        {
            if (string.IsNullOrWhiteSpace(gebruikersnaam))
                throw new ArgumentException("Gebruikersnaam is verplicht.", nameof(gebruikersnaam));
            if (wachtwoord is null)
                throw new ArgumentNullException(nameof(wachtwoord));
            if (owner is null)
                throw new ArgumentNullException(nameof(owner));

            if (!Enum.TryParse<Rol>(rolNaam ?? "", true, out var rol))
                throw new ArgumentException($"Ongeldige rol: {rolNaam}", nameof(rolNaam));

            return new Account(gebruikersnaam, wachtwoord, rol, owner);
        }
    }
}