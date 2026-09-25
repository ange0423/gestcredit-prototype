using GestCredit.Prototype.Exceptions;

namespace GestCredit.Prototype.Modeles;

public class DemandeCredit
{
    public int Id { get; }
    public int ClientId { get; }
    public decimal Montant { get; }
    public StatutDemande Statut { get; private set; }

    // Transitions légales : statut actuel -> statuts autorisés
    private static readonly Dictionary<StatutDemande, StatutDemande[]> TransitionsAutorisees = new()
    {
        [StatutDemande.Brouillon] = new[] { StatutDemande.Soumise },
        [StatutDemande.Soumise] = new[] { StatutDemande.EnAnalyse },
        [StatutDemande.EnAnalyse] = new[] { StatutDemande.Approuvee, StatutDemande.Rejetee },
        [StatutDemande.Approuvee] = Array.Empty<StatutDemande>(),
        [StatutDemande.Rejetee] = Array.Empty<StatutDemande>(),
    };

    // Constructeur utilisé à la fois pour créer une nouvelle demande ET pour la désérialisation JSON
    // (System.Text.Json reconnaît ce constructeur car ses paramètres correspondent aux propriétés)
    public DemandeCredit(int id, int clientId, decimal montant, StatutDemande statut = StatutDemande.Brouillon)
    {
        if (montant <= 0)
            throw new ArgumentException("Le montant doit être positif.", nameof(montant));

        Id = id;
        ClientId = clientId;
        Montant = montant;
        Statut = statut;
    }

    public void ChangerStatut(StatutDemande nouveauStatut)
    {
        bool transitionAutorisee = TransitionsAutorisees[Statut].Contains(nouveauStatut);

        if (!transitionAutorisee)
            throw new TransitionStatutInvalideException(Statut, nouveauStatut);

        Statut = nouveauStatut;
    }
}