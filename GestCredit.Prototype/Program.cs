// GestCredit.Prototype - Jour 10
// Exercice intégrateur Semaine 2 : persistance JSON asynchrone + menu complet
#nullable enable

using GestCredit.Prototype.Exceptions;
using GestCredit.Prototype.Modeles;
using GestCredit.Prototype.Services;

var depot = new DepotDonnees();

// Chargement initial asynchrone (await direct : les top-level statements supportent async)
List<Client> clients = await depot.ChargerClientsAsync();
List<DemandeCredit> demandes = await depot.ChargerDemandesAsync();

Console.WriteLine($"Données chargées : {clients.Count} client(s), {demandes.Count} demande(s).");

bool continuer = true;

while (continuer)
{
    Console.WriteLine();
    Console.WriteLine("=== GestCredit ===");
    Console.WriteLine("1. Lister les clients");
    Console.WriteLine("2. Ajouter un client");
    Console.WriteLine("3. Lister les demandes");
    Console.WriteLine("4. Ajouter une demande");
    Console.WriteLine("5. Changer le statut d'une demande");
    Console.WriteLine("6. Rapport (encours par statut, top 5, moyenne par ville)");
    Console.WriteLine("7. Quitter");
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
            await AjouterClientAsync(clients, depot);
            break;

        case 3:
            ListerDemandes(demandes);
            break;

        case 4:
            await AjouterDemandeAsync(demandes, clients, depot);
            break;

        case 5:
            await ChangerStatutDemandeAsync(demandes, depot);
            break;

        case 6:
            AfficherRapport(demandes, clients);
            break;

        case 7:
            continuer = false;
            Console.WriteLine("Au revoir !");
            break;

        default:
            Console.WriteLine("Choix invalide.");
            break;
    }
}


// ===================== Méthodes =====================

static void ListerClients(List<Client> clients)
{
    if (!clients.Any())
    {
        Console.WriteLine("Aucun client enregistré.");
        return;
    }

    foreach (var c in clients.OrderBy(c => c.Nom))
        Console.WriteLine($"  {c}");
}

static async Task AjouterClientAsync(List<Client> clients, DepotDonnees depot)
{
    Console.Write("Nom du client : ");
    string? nom = Console.ReadLine();

    Console.Write("Ville : ");
    string? ville = Console.ReadLine();

    int prochainId = clients.Any() ? clients.Max(c => c.Id) + 1 : 1;

    try
    {
        clients.Add(new Client(prochainId, nom ?? "", ville ?? ""));
        await depot.SauvegarderClientsAsync(clients); // sauvegarde immédiate : les données survivent à un redémarrage
        Console.WriteLine($"Client ajouté avec l'id {prochainId}.");
    }
    catch (ArgumentException ex)
    {
        Console.WriteLine($"Erreur : {ex.Message}");
    }
}

static void ListerDemandes(List<DemandeCredit> demandes)
{
    if (!demandes.Any())
    {
        Console.WriteLine("Aucune demande enregistrée.");
        return;
    }

    foreach (var d in demandes.OrderBy(d => d.Id))
        Console.WriteLine($"  Demande #{d.Id} - Client {d.ClientId} - {d.Montant:N0} FCFA - {d.Statut}");
}

static async Task AjouterDemandeAsync(List<DemandeCredit> demandes, List<Client> clients, DepotDonnees depot)
{
    if (!clients.Any())
    {
        Console.WriteLine("Aucun client enregistré. Ajoutez d'abord un client.");
        return;
    }

    Console.Write("Id du client : ");
    if (!int.TryParse(Console.ReadLine(), out int clientId) || !clients.Any(c => c.Id == clientId))
    {
        Console.WriteLine("Erreur : id de client invalide.");
        return;
    }

    Console.Write("Montant : ");
    if (!decimal.TryParse(Console.ReadLine(), out decimal montant))
    {
        Console.WriteLine("Erreur : montant invalide.");
        return;
    }

    int prochainId = demandes.Any() ? demandes.Max(d => d.Id) + 1 : 1;

    try
    {
        demandes.Add(new DemandeCredit(prochainId, clientId, montant));
        await depot.SauvegarderDemandesAsync(demandes);
        Console.WriteLine($"Demande #{prochainId} ajoutée.");
    }
    catch (ArgumentException ex)
    {
        Console.WriteLine($"Erreur : {ex.Message}");
    }
}

static async Task ChangerStatutDemandeAsync(List<DemandeCredit> demandes, DepotDonnees depot)
{
    Console.Write("Id de la demande : ");
    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        Console.WriteLine("Erreur : id invalide.");
        return;
    }

    DemandeCredit? demande = demandes.FirstOrDefault(d => d.Id == id);
    if (demande is null)
    {
        Console.WriteLine("Aucune demande avec cet id.");
        return;
    }

    Console.WriteLine("Statuts possibles : Brouillon, Soumise, EnAnalyse, Approuvee, Rejetee");
    Console.Write("Nouveau statut : ");
    string? saisie = Console.ReadLine();

    if (!Enum.TryParse<StatutDemande>(saisie, ignoreCase: true, out StatutDemande nouveauStatut))
    {
        Console.WriteLine("Erreur : statut invalide.");
        return;
    }

    try
    {
        demande.ChangerStatut(nouveauStatut);
        await depot.SauvegarderDemandesAsync(demandes);
        Console.WriteLine($"Statut mis à jour : {demande.Statut}");
    }
    catch (TransitionStatutInvalideException ex)
    {
        Console.WriteLine($"Erreur : {ex.Message}");
    }
}

static void AfficherRapport(List<DemandeCredit> demandes, List<Client> clients)
{
    if (!demandes.Any())
    {
        Console.WriteLine("Aucune demande pour générer un rapport.");
        return;
    }

    Console.WriteLine();
    Console.WriteLine("--- Encours par statut ---");
    var parStatut = demandes
        .GroupBy(d => d.Statut)
        .Select(g => new { Statut = g.Key, Total = g.Sum(d => d.Montant) })
        .OrderByDescending(x => x.Total);

    foreach (var ligne in parStatut)
        Console.WriteLine($"  {ligne.Statut,-12} : {ligne.Total,12:N0} FCFA");

    Console.WriteLine();
    Console.WriteLine("--- Top 5 des plus gros montants ---");
    foreach (var d in demandes.OrderByDescending(d => d.Montant).Take(5))
        Console.WriteLine($"  Demande #{d.Id} : {d.Montant:N0} FCFA");

    Console.WriteLine();
    Console.WriteLine("--- Montant moyen par ville ---");
    var parVille = demandes
        .Join(clients, d => d.ClientId, c => c.Id, (d, c) => new { c.Ville, d.Montant })
        .GroupBy(x => x.Ville)
        .Select(g => new { Ville = g.Key, Moyenne = g.Average(x => x.Montant) })
        .OrderByDescending(x => x.Moyenne);

    foreach (var ligne in parVille)
        Console.WriteLine($"  {ligne.Ville,-12} : {ligne.Moyenne,12:N0} FCFA");
}