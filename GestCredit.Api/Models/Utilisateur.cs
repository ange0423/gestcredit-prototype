namespace GestCredit.Api.Models;

public class Utilisateur
{
    public int Id { get; set; }
    public string NomUtilisateur { get; set; } = string.Empty;
    public string MotDePasseHash { get; set; } = string.Empty; // jamais le mot de passe en clair

    // Relation N-N avec Role (skip navigation EF Core) : reflète exactement
    // la table de jonction UtilisateurRole du Jour 11-12 (SQL).
    public ICollection<Role> Roles { get; set; } = new List<Role>();
}
