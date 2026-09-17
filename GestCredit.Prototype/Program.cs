// GestCredit.Prototype - Jour 2
// Calculateur de mensualité de prêt (montant, taux annuel, durée)
// Règle : decimal pour tout montant financier, jamais double

Console.WriteLine("=== Calculateur de mensualité de prêt ===");
Console.WriteLine();

// --- Saisie du montant ---
decimal montant;
while (true)
{
    Console.Write("Montant du prêt (FCFA) : ");
    string? saisieMontant = Console.ReadLine();

    if (!decimal.TryParse(saisieMontant, out montant))
    {
        Console.WriteLine("Erreur : veuillez entrer un nombre valide.");
        continue;
    }

    if (montant <= 0)
    {
        Console.WriteLine("Erreur : le montant doit être positif.");
        continue;
    }

    break; // saisie valide, on sort de la boucle
}

// --- Saisie du taux annuel ---
decimal tauxAnnuel;
while (true)
{
    Console.Write("Taux annuel (%) : ");
    string? saisieTaux = Console.ReadLine();

    if (!decimal.TryParse(saisieTaux, out tauxAnnuel))
    {
        Console.WriteLine("Erreur : veuillez entrer un nombre valide.");
        continue;
    }

    if (tauxAnnuel <= 0)
    {
        Console.WriteLine("Erreur : le taux doit être positif.");
        continue;
    }

    break;
}

// --- Saisie de la durée ---
int duree;
while (true)
{
    Console.Write("Durée (mois) : ");
    string? saisieDuree = Console.ReadLine();

    if (!int.TryParse(saisieDuree, out duree))
    {
        Console.WriteLine("Erreur : veuillez entrer un nombre entier valide.");
        continue;
    }

    if (duree <= 0)
    {
        Console.WriteLine("Erreur : la durée doit être positive.");
        continue;
    }

    break;
}

// --- Calcul de la mensualité ---
// Formule des annuités constantes :
// M = Montant * tauxMensuel / (1 - (1 + tauxMensuel)^(-duree))

decimal tauxMensuel = tauxAnnuel / 12 / 100;

// Math.Pow travaille en double, on convertit puis on reconvertit en decimal
double facteur = 1 - Math.Pow(1 + (double)tauxMensuel, -duree);
decimal mensualite = montant * tauxMensuel / (decimal)facteur;

decimal coutTotal = mensualite * duree;
decimal interets = coutTotal - montant;

// --- Affichage formaté à 2 décimales ---
Console.WriteLine();
Console.WriteLine("=== Résultat ===");
Console.WriteLine($"Mensualité   : {mensualite:N2} FCFA");
Console.WriteLine($"Coût total   : {coutTotal:N2} FCFA");
Console.WriteLine($"Intérêts     : {interets:N2} FCFA");