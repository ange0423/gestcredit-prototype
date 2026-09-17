// GestCredit.Prototype - Jour 3
// Refactoring du Jour 2 en méthodes courtes + switch expression

decimal montant = SaisirMontant();
decimal tauxAnnuel = SaisirTauxAnnuel();
int duree = SaisirDuree();

string categorie = DeterminerCategorie(montant);
Console.WriteLine($"Catégorie du prêt : {categorie}");

(decimal mensualite, decimal coutTotal, decimal interets) = CalculerPret(montant, tauxAnnuel, duree);

AfficherResultat(mensualite, coutTotal, interets);


// ===================== Méthodes =====================

static decimal SaisirMontant()
{
    while (true)
    {
        Console.Write("Montant du prêt (FCFA) : ");
        string? saisie = Console.ReadLine();

        if (!decimal.TryParse(saisie, out decimal montant))
        {
            Console.WriteLine("Erreur : veuillez entrer un nombre valide.");
            continue;
        }

        if (montant <= 0)
        {
            Console.WriteLine("Erreur : le montant doit être positif.");
            continue;
        }

        return montant;
    }
}

static decimal SaisirTauxAnnuel()
{
    while (true)
    {
        Console.Write("Taux annuel (%) : ");
        string? saisie = Console.ReadLine();

        if (!decimal.TryParse(saisie, out decimal taux))
        {
            Console.WriteLine("Erreur : veuillez entrer un nombre valide.");
            continue;
        }

        if (taux <= 0)
        {
            Console.WriteLine("Erreur : le taux doit être positif.");
            continue;
        }

        return taux;
    }
}

static int SaisirDuree()
{
    while (true)
    {
        Console.Write("Durée (mois) : ");
        string? saisie = Console.ReadLine();

        if (!int.TryParse(saisie, out int duree))
        {
            Console.WriteLine("Erreur : veuillez entrer un nombre entier valide.");
            continue;
        }

        if (duree <= 0)
        {
            Console.WriteLine("Erreur : la durée doit être positive.");
            continue;
        }

        return duree;
    }
}

// switch expression : classe le montant en catégorie
static string DeterminerCategorie(decimal montant) => montant switch
{
    < 500_000 => "Micro-crédit",
    < 5_000_000 => "Standard",
    _ => "Grand compte"
};

static (decimal mensualite, decimal coutTotal, decimal interets) CalculerPret(decimal montant, decimal tauxAnnuel, int duree)
{
    decimal tauxMensuel = tauxAnnuel / 12 / 100;
    double facteur = 1 - Math.Pow(1 + (double)tauxMensuel, -duree);
    decimal mensualite = montant * tauxMensuel / (decimal)facteur;

    decimal coutTotal = mensualite * duree;
    decimal interets = coutTotal - montant;

    return (mensualite, coutTotal, interets);
}

static void AfficherResultat(decimal mensualite, decimal coutTotal, decimal interets)
{
    Console.WriteLine();
    Console.WriteLine("=== Résultat ===");
    Console.WriteLine($"Mensualité   : {mensualite:N2} FCFA");
    Console.WriteLine($"Coût total   : {coutTotal:N2} FCFA");
    Console.WriteLine($"Intérêts     : {interets:N2} FCFA");
}