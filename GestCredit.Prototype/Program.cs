// GestCredit.Prototype - Jour 8
// Exceptions personnalisées, enum, transitions de statut, nullable reference types
#nullable enable

var demande = new DemandeCredit(id: 1, montant: 2_000_000m);
Console.WriteLine($"Statut initial : {demande.Statut}");

// Transition légale : Brouillon -> Soumise
try
{
    demande.ChangerStatut(StatutDemande.Soumise);
    Console.WriteLine($"Nouveau statut : {demande.Statut}");
}
catch (TransitionStatutInvalideException ex)
{
    Console.WriteLine($"Erreur : {ex.Message}");
}

// Transition illégale volontaire : Soumise -> Approuvee (on saute "En analyse")
try
{
    demande.ChangerStatut(StatutDemande.Approuvee);
    Console.WriteLine($"Nouveau statut : {demande.Statut}");
}
catch (TransitionStatutInvalideException ex)
{
    Console.WriteLine($"Erreur attendue : {ex.Message}");
}

// Transition légale : Soumise -> EnAnalyse -> Approuvee
demande.ChangerStatut(StatutDemande.EnAnalyse);
Console.WriteLine($"Nouveau statut : {demande.Statut}");

demande.ChangerStatut(StatutDemande.Approuvee);
Console.WriteLine($"Nouveau statut : {demande.Statut}");


// ===================== Enum =====================

enum StatutDemande
{
    Brouillon,
    Soumise,
    EnAnalyse,
    Approuvee,
    Rejetee
}


// ===================== Exception personnalisée =====================

class TransitionStatutInvalideException : Exception
{
    public TransitionStatutInvalideException(StatutDemande statutActuel, StatutDemande statutCible)
        : base($"Transition invalide : impossible de passer de '{statutActuel}' à '{statutCible}'.")
    {
    }
}


// ===================== Classe métier =====================

class DemandeCredit
{
    public int Id { get; }
    public decimal Montant { get; }
    public StatutDemande Statut { get; private set; } // set privé : seule la classe elle-même peut le modifier

    public DemandeCredit(int id, decimal montant)
    {
        if (montant <= 0)
            throw new ArgumentException("Le montant doit être positif.", nameof(montant));

        Id = id;
        Montant = montant;
        Statut = StatutDemande.Brouillon; // état initial obligatoire
    }

    // Dictionnaire des transitions légales : statut actuel -> statuts autorisés
    private static readonly Dictionary<StatutDemande, StatutDemande[]> TransitionsAutorisees = new()
    {
        [StatutDemande.Brouillon] = new[] { StatutDemande.Soumise },
        [StatutDemande.Soumise] = new[] { StatutDemande.EnAnalyse },
        [StatutDemande.EnAnalyse] = new[] { StatutDemande.Approuvee, StatutDemande.Rejetee },
        [StatutDemande.Approuvee] = Array.Empty<StatutDemande>(), // statut final
        [StatutDemande.Rejetee] = Array.Empty<StatutDemande>(),  // statut final
    };

    public void ChangerStatut(StatutDemande nouveauStatut)
    {
        bool transitionAutorisee = TransitionsAutorisees[Statut].Contains(nouveauStatut);

        if (!transitionAutorisee)
            throw new TransitionStatutInvalideException(Statut, nouveauStatut);

        Statut = nouveauStatut;
    }
}