namespace GestCredit.Api.Dtos;

public record AuthResponseDto(string Token, DateTime ExpiresAt, string NomUtilisateur, List<string> Roles);
