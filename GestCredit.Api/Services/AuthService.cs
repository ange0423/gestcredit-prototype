using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GestCredit.Api.Data;
using GestCredit.Api.Dtos;
using GestCredit.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace GestCredit.Api.Services;

public class AuthService : IAuthService
{
    private readonly GestCreditDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(GestCreditDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto?> RegisterAsync(RegisterDto dto)
    {
        bool dejaPris = await _context.Utilisateurs.AnyAsync(u => u.NomUtilisateur == dto.NomUtilisateur);
        if (dejaPris) return null;

        Role? role = await _context.Roles.FirstOrDefaultAsync(r => r.Nom == dto.Role);
        if (role is null) return null;

        var utilisateur = new Utilisateur
        {
            NomUtilisateur = dto.NomUtilisateur,
            // Hachage avec sel aléatoire intégré (BCrypt) : jamais de mot de passe en clair stocké.
            MotDePasseHash = BCrypt.Net.BCrypt.HashPassword(dto.MotDePasse),
            Roles = new List<Role> { role }
        };

        _context.Utilisateurs.Add(utilisateur);
        await _context.SaveChangesAsync();

        return GenererReponse(utilisateur, new[] { role.Nom });
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        Utilisateur? utilisateur = await _context.Utilisateurs
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.NomUtilisateur == dto.NomUtilisateur);

        if (utilisateur is null) return null;
        if (!BCrypt.Net.BCrypt.Verify(dto.MotDePasse, utilisateur.MotDePasseHash)) return null;

        return GenererReponse(utilisateur, utilisateur.Roles.Select(r => r.Nom));
    }

    private AuthResponseDto GenererReponse(Utilisateur utilisateur, IEnumerable<string> roles)
    {
        List<string> rolesList = roles.ToList();

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, utilisateur.Id.ToString()),
            new(ClaimTypes.Name, utilisateur.NomUtilisateur)
        };
        claims.AddRange(rolesList.Select(r => new Claim(ClaimTypes.Role, r)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        DateTime expires = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:ExpiresMinutes"]!));

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds);

        return new AuthResponseDto(
            new JwtSecurityTokenHandler().WriteToken(token),
            expires,
            utilisateur.NomUtilisateur,
            rolesList);
    }
}
