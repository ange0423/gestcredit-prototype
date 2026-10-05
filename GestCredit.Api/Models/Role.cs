namespace GestCredit.Api.Models;

public class Role
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;

    public ICollection<Utilisateur> Utilisateurs { get; set; } = new List<Utilisateur>();
}
