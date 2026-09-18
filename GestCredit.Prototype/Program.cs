// GestCredit.Prototype - Jour 9
// Rapport LINQ avancé : GroupBy, Join, SelectMany, Aggregate, Skip/Take
#nullable enable

// ===================== Génération de données de test =====================

var villes = new[] { "Dakar", "Thiès", "Saint-Louis", "Ziguinchor", "Kaolack" };
var random = new Random(42); // seed fixe pour des résultats reproductibles

var clients = Enumerable.Range(1, 10)
    .Select(id => new Client(id, $"Client {id}", villes[random.Next(villes.Length)]))
    .ToList();

var statuts = new[] { "Brouillon", "Soumise", "EnAnalyse", "Approuvee", "Rejetee" };

var demandes = Enumerable.Range(1, 20)
    .Select(id => new Demande(
        id,
        ClientId: clients[random.Next(clients.Count)].Id,
        Montant: random.Next(200_000, 10_000_000),
        Statut: statuts[random.Next(statuts.Length)]))
    .ToList();


// ===================== Rapport =====================

Console.WriteLine("=== Encours par statut ===");
var encoursParStatut = demandes
    .GroupBy(d => d.Statut)
    .Select(g => new { Statut = g.Key, Total = g.Sum(d => d.Montant), Nombre = g.Count() })
    .OrderByDescending(x => x.Total);

foreach (var ligne in encoursParStatut)
    Console.WriteLine($"  {ligne.Statut,-12} : {ligne.Total,12:N0} FCFA ({ligne.Nombre} demande(s))");


Console.WriteLine();
Console.WriteLine("=== Top 5 des plus gros montants ===");
var top5 = demandes
    .OrderByDescending(d => d.Montant)
    .Take(5);

foreach (var d in top5)
    Console.WriteLine($"  Demande #{d.Id} : {d.Montant:N0} FCFA");


Console.WriteLine();
Console.WriteLine("=== Montant moyen par ville (jointure client <-> demande) ===");
var moyenneParVille = demandes
    .Join(clients,
        demande => demande.ClientId,
        client => client.Id,
        (demande, client) => new { client.Ville, demande.Montant })
    .GroupBy(x => x.Ville)
    .Select(g => new { Ville = g.Key, Moyenne = g.Average(x => x.Montant) })
    .OrderByDescending(x => x.Moyenne);

foreach (var ligne in moyenneParVille)
    Console.WriteLine($"  {ligne.Ville,-12} : {ligne.Moyenne,12:N0} FCFA en moyenne");


Console.WriteLine();
Console.WriteLine("=== Clients sans aucune demande ===");
var idsClientsAvecDemande = demandes.Select(d => d.ClientId).Distinct();
var clientsSansDemande = clients.Where(c => !idsClientsAvecDemande.Contains(c.Id));

if (!clientsSansDemande.Any())
    Console.WriteLine("  Aucun (tous les clients ont au moins une demande).");
else
    foreach (var c in clientsSansDemande)
        Console.WriteLine($"  [{c.Id}] {c.Nom} ({c.Ville})");


Console.WriteLine();
Console.WriteLine("=== Encours total (Aggregate) ===");
decimal encoursTotal = demandes.Aggregate(0m, (total, d) => total + d.Montant);
Console.WriteLine($"  Total : {encoursTotal:N0} FCFA");


// ===================== Modèles =====================

record Client(int Id, string Nom, string Ville);
record Demande(int Id, int ClientId, decimal Montant, string Statut);