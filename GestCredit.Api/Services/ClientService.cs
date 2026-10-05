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

    public async Task<PagedResult<ClientDto>> GetClientsAsync(ClientQueryParameters query)
    {
        IQueryable<Client> clients = _context.Clients.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Ville))
        {
            clients = clients.Where(c => c.Ville == query.Ville);
        }

        clients = query.TriPar?.ToLower() switch
        {
            "ville" => query.TriDescendant
                ? clients.OrderByDescending(c => c.Ville)
                : clients.OrderBy(c => c.Ville),
            _ => query.TriDescendant
                ? clients.OrderByDescending(c => c.Nom)
                : clients.OrderBy(c => c.Nom)
        };

        int totalCount = await clients.CountAsync();

        List<ClientDto> items = await clients
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(c => new ClientDto(c.Id, c.Nom, c.Ville, c.Email))
            .ToListAsync();

        return new PagedResult<ClientDto>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<ClientDto?> GetClientAsync(int id)
    {
        Client? client = await _context.Clients
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

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
        if (client is null) return false;

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
