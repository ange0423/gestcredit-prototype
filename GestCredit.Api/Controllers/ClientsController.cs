using GestCredit.Api.Data;
using GestCredit.Api.Dtos;
using GestCredit.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestCredit.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly GestCreditDbContext _context;

    public ClientsController(GestCreditDbContext context)
    {
        _context = context;
    }

    // GET api/clients
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClientDto>>> GetClients()
    {
        // AsNoTracking + projection directe en DTO : lecture seule, pas de
        // suivi de changement inutile, et surtout pas de risque de N+1
        // puisqu'on ne charge aucune propriété de navigation ici.
        List<ClientDto> clients = await _context.Clients
            .AsNoTracking()
            .Select(c => new ClientDto(c.Id, c.Nom, c.Ville, c.Email))
            .ToListAsync();

        return Ok(clients);
    }

    // GET api/clients/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ClientDto>> GetClient(int id)
    {
        Client? client = await _context.Clients
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (client is null) return NotFound(); // 404, jamais une exception -> 500

        return Ok(new ClientDto(client.Id, client.Nom, client.Ville, client.Email));
    }

    // POST api/clients
    [HttpPost]
    public async Task<ActionResult<ClientDto>> CreateClient(CreateClientDto dto)
    {
        var client = new Client { Nom = dto.Nom, Ville = dto.Ville, Email = dto.Email };

        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        var result = new ClientDto(client.Id, client.Nom, client.Ville, client.Email);

        // 201 + en-tête Location pointant vers GetClient(id) : convention REST attendue.
        return CreatedAtAction(nameof(GetClient), new { id = client.Id }, result);
    }

    // PUT api/clients/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateClient(int id, UpdateClientDto dto)
    {
        Client? client = await _context.Clients.FindAsync(id);
        if (client is null) return NotFound();

        client.Nom = dto.Nom;
        client.Ville = dto.Ville;
        client.Email = dto.Email;

        await _context.SaveChangesAsync();
        return NoContent(); // 204
    }

    // DELETE api/clients/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteClient(int id)
    {
        Client? client = await _context.Clients.FindAsync(id);
        if (client is null) return NotFound();

        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();
        return NoContent(); // 204
    }
}