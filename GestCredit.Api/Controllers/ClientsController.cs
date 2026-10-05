using FluentValidation;
using FluentValidation.Results;
using GestCredit.Api.Dtos;
using GestCredit.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestCredit.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly IClientService _clientService;
    private readonly IValidator<CreateClientDto> _createValidator;
    private readonly IValidator<UpdateClientDto> _updateValidator;

    public ClientsController(
        IClientService clientService,
        IValidator<CreateClientDto> createValidator,
        IValidator<UpdateClientDto> updateValidator)
    {
        _clientService = clientService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    // Le controller ne fait plus que : valider l'entrée, appeler le service,
    // traduire le résultat en code HTTP. Toujours aucun try/catch (Jour 18).

    // GET api/clients?ville=Douala&triPar=ville&triDescendant=true&page=1&pageSize=10
    [HttpGet]
    public async Task<ActionResult<PagedResult<ClientDto>>> GetClients([FromQuery] ClientQueryParameters query)
    {
        PagedResult<ClientDto> result = await _clientService.GetClientsAsync(query);
        return Ok(result);
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
        ValidationResult validation = await _createValidator.ValidateAsync(dto);
        if (!validation.IsValid) return BadRequestFrom(validation);

        var result = await _clientService.CreateClientAsync(dto);
        return CreatedAtAction(nameof(GetClient), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateClient(int id, UpdateClientDto dto)
    {
        ValidationResult validation = await _updateValidator.ValidateAsync(dto);
        if (!validation.IsValid) return BadRequestFrom(validation);

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

    // 400 structuré, erreurs groupées par champ : { "errors": { "Nom": ["..."] } }
    private ActionResult BadRequestFrom(ValidationResult validation)
    {
        foreach (ValidationFailure error in validation.Errors)
        {
            ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }
        return ValidationProblem(ModelState);
    }
}
