namespace GestCredit.Api.Dtos;

// Role : "Agent", "Analyste" ou "Administrateur" (doit exister dans la table Role)
public record RegisterDto(string NomUtilisateur, string MotDePasse, string Role);
