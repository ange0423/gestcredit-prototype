namespace GestCredit.Api.Models;

// Entité EF Core : propriétés mutables (get/set), contrairement au Client "prototype"
// des Semaines 1-2. EF a besoin de pouvoir matérialiser un objet depuis une ligne SQL ;
// l'encapsulation stricte (constructeur validant, set privé) reviendra en Semaine 5
// via les DTO et la couche Application, sans jamais exposer cette entité telle quelle.
public class Client
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Ville { get; set; } = string.Empty;
    public string? Email { get; set; }

    public ICollection<DemandeCredit> Demandes { get; set; } = new List<DemandeCredit>();
}