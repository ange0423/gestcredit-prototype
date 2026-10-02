using GestCredit.Api.Data;
using GestCredit.Api.Dtos;
using GestCredit.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GestCredit.Api.Services;

// Enregistré en Scoped (voir Program.cs) : une instance par requête HTTP,
// alignée sur le cycle de vie du DbContext lui-même (lui aussi Scoped par défaut).
public class ClientService : IClientService
{
    private readonly GestCreditDbContext _context;

    public ClientService(GestCreditDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ClientDto>> GetClientsAsync()
    {
        return await _context.Clients
            .AsNoTracking()
            .Select(c => new ClientDto(c.Id, c.Nom, c.Ville, c.Email))
            .ToListAsync();
    }

    public async Task<ClientDto?> GetClientAsync(int id)
    {
        Client? client = await _context.Clients
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        // Pas de try/catch ici : une absence de résultat n'est pas une erreur,
        // c'est un cas métier normal. On renvoie simplement null, et c'est au
        // controller de traduire ça en 404 (voir ClientsController).
        return client is null ? null : new ClientDto(client.Id, client.Nom, client.Ville, client.Email);
    }

    public async Task<ClientDto> CreateClientAsync(CreateClientDto dto)
    {
        var client = new Client { Nom = dto.Nom, Ville = dto.Ville, Email = dto.Email };

        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        return new ClientDto(client.Id, client.Nom, client.Ville, client.Email);
    }

    public async Task<bool> UpdateClientAsync(int id, UpdateClientDto dto)
    {
        Client? client = await _context.Clients.FindAsync(id);
        if (client is null) return false; // le controller traduit ça en 404

        client.Nom = dto.Nom;
        client.Ville = dto.Ville;
        client.Email = dto.Email;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteClientAsync(int id)
    {
        Client? client = await _context.Clients.FindAsync(id);
        if (client is null) return false;

        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();
        return true;
    }
}