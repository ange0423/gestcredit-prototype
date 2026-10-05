namespace GestCredit.Api.Dtos;

// DTO : ne jamais exposer l'entité EF directement (elle porterait la propriété
// de navigation Demandes, et couplerait le contrat de l'API au schéma de la base).
public record ClientDto(int Id, string Nom, string Ville, string? Email);

public record CreateClientDto(string Nom, string Ville, string? Email);

public record UpdateClientDto(string Nom, string Ville, string? Email);