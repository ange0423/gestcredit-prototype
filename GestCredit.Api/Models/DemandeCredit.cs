namespace GestCredit.Api.Models;

public class DemandeCredit
{
    public int Id { get; set; }

    public int ClientId { get; set; }
    public Client? Client { get; set; } // propriété de navigation

    public decimal Montant { get; set; }
    public decimal TauxAnnuel { get; set; }
    public int DureeMois { get; set; }
    public StatutDemande Statut { get; set; } = StatutDemande.Brouillon;
    public DateTime DateCreation { get; set; } = DateTime.UtcNow;
}