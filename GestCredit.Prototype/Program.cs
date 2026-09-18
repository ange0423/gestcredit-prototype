// GestCredit.Prototype - Jour 7
// Interface ICalculateurInteret avec plusieurs implémentations (polymorphisme)

var calculateurs = new List<ICalculateurInteret>
{
    new CalculateurInteretSimple(),
    new CalculateurInteretCompose(),
    new CalculateurInteretDegressif(),
};

decimal montant = 1_000_000m;
decimal tauxAnnuel = 10m;
int dureeAnnees = 3;

// La boucle ne connaît AUCUNE classe concrète : elle ne manipule que l'interface.
// Ajouter une 4e implémentation à la liste ci-dessus ne demandera aucune modification ici.
foreach (ICalculateurInteret calculateur in calculateurs)
{
    decimal interets = calculateur.CalculerInterets(montant, tauxAnnuel, dureeAnnees);
    Console.WriteLine($"{calculateur.GetType().Name} : {interets:N2} FCFA d'intérêts");
}


// ===================== Interface =====================

interface ICalculateurInteret
{
    decimal CalculerInterets(decimal montant, decimal tauxAnnuel, int dureeAnnees);
}


// ===================== Implémentations =====================

// Intérêt simple : interets = montant * taux * durée (le taux ne s'applique qu'au capital initial)
class CalculateurInteretSimple : ICalculateurInteret
{
    public decimal CalculerInterets(decimal montant, decimal tauxAnnuel, int dureeAnnees)
    {
        decimal taux = tauxAnnuel / 100;
        return montant * taux * dureeAnnees;
    }
}

// Intérêt composé : le capital croît chaque année, les intérêts se recalculent sur le nouveau capital
class CalculateurInteretCompose : ICalculateurInteret
{
    public decimal CalculerInterets(decimal montant, decimal tauxAnnuel, int dureeAnnees)
    {
        decimal taux = tauxAnnuel / 100;
        decimal capitalFinal = montant * (decimal)Math.Pow(1 + (double)taux, dureeAnnees);
        return capitalFinal - montant;
    }
}

// Intérêt dégressif : le taux s'applique sur un capital qui diminue chaque année (ex. remboursement linéaire)
class CalculateurInteretDegressif : ICalculateurInteret
{
    public decimal CalculerInterets(decimal montant, decimal tauxAnnuel, int dureeAnnees)
    {
        decimal taux = tauxAnnuel / 100;
        decimal capitalRestant = montant;
        decimal amortissementAnnuel = montant / dureeAnnees;
        decimal totalInterets = 0;

        for (int annee = 1; annee <= dureeAnnees; annee++)
        {
            totalInterets += capitalRestant * taux;
            capitalRestant -= amortissementAnnuel;
        }

        return totalInterets;
    }
}