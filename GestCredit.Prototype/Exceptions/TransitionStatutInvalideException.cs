using GestCredit.Prototype.Modeles;

namespace GestCredit.Prototype.Exceptions;

public class TransitionStatutInvalideException : Exception
{
    public TransitionStatutInvalideException(StatutDemande statutActuel, StatutDemande statutCible)
        : base($"Transition invalide : impossible de passer de '{statutActuel}' à '{statutCible}'.")
    {
    }
}