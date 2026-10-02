using GestCredit.Api.Dtos;
using GestCredit.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestCredit.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly IClientService _clientService;

    public ClientsController(IClientService clientService)
    {
        _clientService = clientService;
    }

    // Le controller ne fait plus que : appeler le service, traduire le résultat
    // en code HTTP. Aucune logique métier, aucun accès direct au DbContext,
    // et surtout aucun try/catch : les erreurs inattendues sont interceptées
    // plus haut par le middleware d'exception globale (Jour 18).

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClientDto>>> GetClients()
    {
        var clients = await _clientService.GetClientsAsync();
        return Ok(clients);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ClientDto>> GetClient(int id)
    {
        var client = await _clientService.GetClientAsync(id);
        if (client is null) return NotFound();
        return Ok(client);
    }

    [HttpPost]
    public async Task<ActionResult<ClientDto>> CreateClient(CreateClientDto dto)
    {
        var result = await _clientService.CreateClientAsync(dto);
        return CreatedAtAction(nameof(GetClient), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateClient(int id, UpdateClientDto dto)
    {
        bool updated = await _clientService.UpdateClientAsync(id, dto);
        if (!updated) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteClient(int id)
    {
        bool deleted = await _clientService.DeleteClientAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}