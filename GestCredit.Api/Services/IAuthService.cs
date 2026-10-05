using GestCredit.Api.Dtos;

namespace GestCredit.Api.Services;

public interface IAuthService
{
    // null = nom d'utilisateur déjà pris, ou rôle inconnu -> le controller traduit en 400
    Task<AuthResponseDto?> RegisterAsync(RegisterDto dto);

    // null = identifiants invalides -> le controller traduit en 401
    Task<AuthResponseDto?> LoginAsync(LoginDto dto);
}
