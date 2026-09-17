// GestCredit.Prototype - Jour 4
// Gestion d'une liste de clients en mémoire, entièrement via LINQ

var clients = new List<Client>
{
    new Client(1, "Amadou Diallo", "Dakar"),
    new Client(2, "Fatou Ndiaye", "Thiès"),
    new Client(3, "Moussa Kane", "Dakar"),
    new Client(4, "Aïssatou Ba", "Saint-Louis"),
    new Client(5, "Ibrahima Sow", "Dakar"),
};

bool continuer = true;

while (continuer)
{
    Console.WriteLine();
    Console.WriteLine("=== Gestion des clients ===");
    Console.WriteLine("1. Lister les clients (triés par nom)");
    Console.WriteLine("2. Ajouter un client");
    Console.WriteLine("3. Supprimer un client par id");
    Console.WriteLine("4. Rechercher par fragment de nom");
    Console.WriteLine("5. Compter les clients");
    Console.WriteLine("6. Quitter");
    Console.Write("Votre choix : ");

    if (!int.TryParse(Console.ReadLine(), out int choix))
    {
        Console.WriteLine("Erreur : entrez un nombre.");
        continue;
    }

    switch (choix)
    {
        case 1:
            ListerClients(clients);
            break;

        case 2:
            AjouterClient(clients);
            break;

        case 3:
            SupprimerClient(clients);
            break;

        case 4:
            RechercherParNom(clients);
            break;

        case 5:
            Console.WriteLine($"Nombre de clients : {clients.Count()}");
            break;

        case 6:
            continuer = false;
            break;

        default:
            Console.WriteLine("Choix invalide.");
            break;
    }
}


// ===================== Méthodes =====================

static void ListerClients(List<Client> clients)
{
    // OrderBy : tri par nom, sans boucle manuelle
    var triés = clients.OrderBy(c => c.Nom);

    if (!triés.Any())
    {
        Console.WriteLine("Aucun client enregistré.");
        return;
    }

    foreach (var c in triés)
        Console.WriteLine($"  [{c.Id}] {c.Nom} - {c.Ville}");
}

static void AjouterClient(List<Client> clients)
{
    Console.Write("Nom du client : ");
    string? nom = Console.ReadLine();

    Console.Write("Ville : ");
    string? ville = Console.ReadLine();

    // Select + calcul du prochain id sans boucle manuelle
    int prochainId = clients.Any() ? clients.Select(c => c.Id).Max() + 1 : 1;

    clients.Add(new Client(prochainId, nom ?? "", ville ?? ""));
    Console.WriteLine($"Client ajouté avec l'id {prochainId}.");
}

static void SupprimerClient(List<Client> clients)
{
    Console.Write("Id du client à supprimer : ");
    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        Console.WriteLine("Erreur : id invalide.");
        return;
    }

    // FirstOrDefault : recherche sans boucle manuelle
    Client? client = clients.FirstOrDefault(c => c.Id == id);

    if (client is null)
    {
        Console.WriteLine("Aucun client avec cet id.");
        return;
    }

    clients.Remove(client);
    Console.WriteLine("Client supprimé.");
}

static void RechercherParNom(List<Client> clients)
{
    Console.Write("Fragment de nom à rechercher : ");
    string? fragment = Console.ReadLine() ?? "";

    // Where + Contains insensible à la casse, sans boucle manuelle
    var résultats = clients
        .Where(c => c.Nom.Contains(fragment, StringComparison.OrdinalIgnoreCase))
        .OrderBy(c => c.Nom);

    if (!résultats.Any())
    {
        Console.WriteLine("Aucun résultat.");
        return;
    }

    foreach (var c in résultats)
        Console.WriteLine($"  [{c.Id}] {c.Nom} - {c.Ville}");
}


// ===================== Modèle =====================

record Client(int Id, string Nom, string Ville);