using System.Text.Json;
using GestCredit.Prototype.Modeles;

namespace GestCredit.Prototype.Services;

// Service de persistance asynchrone en JSON.
// Convention respectée : chaque méthode se termine par "Async" et renvoie une Task.
public class DepotDonnees
{
    private const string FichierClients = "clients.json";
    private const string FichierDemandes = "demandes.json";

    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true // JSON lisible pour inspection manuelle du fichier
    };

    public async Task<List<Client>> ChargerClientsAsync()
    {
        if (!File.Exists(FichierClients))
            return new List<Client>();

        string json = await File.ReadAllTextAsync(FichierClients);

        if (string.IsNullOrWhiteSpace(json))
            return new List<Client>();

        return JsonSerializer.Deserialize<List<Client>>(json, Options) ?? new List<Client>();
    }

    public async Task SauvegarderClientsAsync(List<Client> clients)
    {
        string json = JsonSerializer.Serialize(clients, Options);
        await File.WriteAllTextAsync(FichierClients, json);
    }

    public async Task<List<DemandeCredit>> ChargerDemandesAsync()
    {
        if (!File.Exists(FichierDemandes))
            return new List<DemandeCredit>();

        string json = await File.ReadAllTextAsync(FichierDemandes);

        if (string.IsNullOrWhiteSpace(json))
            return new List<DemandeCredit>();

        return JsonSerializer.Deserialize<List<DemandeCredit>>(json, Options) ?? new List<DemandeCredit>();
    }

    public async Task SauvegarderDemandesAsync(List<DemandeCredit> demandes)
    {
        string json = JsonSerializer.Serialize(demandes, Options);
        await File.WriteAllTextAsync(FichierDemandes, json);
    }
}