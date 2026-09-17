// GestCredit.Prototype - Jour 1
// Menu console à 4 options, qui gère les saisies invalides sans planter

bool continuer = true;

while (continuer)
{
    // Affichage du menu
    Console.WriteLine();
    Console.WriteLine("=== GestCredit - Menu principal ===");
    Console.WriteLine("1. Ajouter un client");
    Console.WriteLine("2. Lister les clients");
    Console.WriteLine("3. Simuler un crédit");
    Console.WriteLine("4. Quitter");
    Console.Write("Votre choix : ");

    // Lecture de la saisie utilisateur (toujours une string, jamais null ici car Console.ReadLine peut renvoyer null en théorie)
    string? saisie = Console.ReadLine();

    // Tentative de conversion en entier, SANS exception si ça échoue
    bool choixValide = int.TryParse(saisie, out int choix);

    if (!choixValide)
    {
        Console.WriteLine("Erreur : veuillez entrer un nombre entre 1 et 4.");
        continue; // on relance la boucle, on redemande
    }

    // Le switch expression / statement gère chaque option
    switch (choix)
    {
        case 1:
            Console.WriteLine("-> [À implémenter] Ajouter un client");
            break;

        case 2:
            Console.WriteLine("-> [À implémenter] Lister les clients");
            break;

        case 3:
            Console.WriteLine("-> [À implémenter] Simuler un crédit");
            break;

        case 4:
            Console.WriteLine("Au revoir !");
            continuer = false; // on sort de la boucle while
            break;

        default:
            Console.WriteLine("Erreur : choix invalide, entrez un nombre entre 1 et 4.");
            break;
    }
}