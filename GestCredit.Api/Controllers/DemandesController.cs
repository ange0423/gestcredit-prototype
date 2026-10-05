using FluentValidation;
using FluentValidation.Results;
using GestCredit.Api.Data;
using GestCredit.Api.Dtos;
using GestCredit.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestCredit.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DemandesController : ControllerBase
{
    private readonly GestCreditDbContext _context;
    private readonly IValidator<CreateDemandeCreditDto> _validator;

    public DemandesController(GestCreditDbContext context, IValidator<CreateDemandeCreditDto> validator)
    {
        _context = context;
        _validator = validator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateDemande(CreateDemandeCreditDto dto)
    {
        ValidationResult validation = await _validator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            foreach (ValidationFailure error in validation.Errors)
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            return ValidationProblem(ModelState);
        }

        bool clientExiste = await _context.Clients.AnyAsync(c => c.Id == dto.ClientId);
        if (!clientExiste) return NotFound($"Client {dto.ClientId} introuvable.");

        var demande = new DemandeCredit
        {
            ClientId = dto.ClientId,
            Montant = dto.Montant,
            TauxAnnuel = dto.TauxAnnuel,
            DureeMois = dto.DureeMois,
            Statut = StatutDemande.Brouillon
        };

        _context.Demandes.Add(demande);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(CreateDemande), new { id = demande.Id }, new { demande.Id, demande.Statut });
    }
}
