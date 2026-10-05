using FluentValidation;
using FluentValidation.Results;
using GestCredit.Api.Data;
using GestCredit.Api.Dtos;
using GestCredit.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestCredit.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // tout endpoint de ce controller exige un token valide (401 sinon)
public class DemandesController : ControllerBase
{
    private readonly GestCreditDbContext _context;
    private readonly IValidator<CreateDemandeCreditDto> _validator;

    private static readonly Dictionary<StatutDemande, StatutDemande[]> TransitionsAutorisees = new()
    {
        [StatutDemande.Brouillon] = new[] { StatutDemande.Soumise },
        [StatutDemande.Soumise] = new[] { StatutDemande.EnAnalyse },
        [StatutDemande.EnAnalyse] = new[] { StatutDemande.Approuvee, StatutDemande.Rejetee },
        [StatutDemande.Approuvee] = Array.Empty<StatutDemande>(),
        [StatutDemande.Rejetee] = Array.Empty<StatutDemande>()
    };

    public DemandesController(GestCreditDbContext context, IValidator<CreateDemandeCreditDto> validator)
    {
        _context = context;
        _validator = validator;
    }

    // Seul un Agent peut déposer une demande.
    [HttpPost]
    [Authorize(Roles = "Agent")]
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

    // Seul un Analyste peut faire avancer le workflow (EnAnalyse -> Approuvee/Rejetee, etc.).
    // Un Agent qui appelle cet endpoint reçoit 403 (authentifié, mais pas le bon rôle).
    [HttpPatch("{id:int}/statut")]
    [Authorize(Roles = "Analyste")]
    public async Task<IActionResult> ChangerStatut(int id, ChangerStatutDto dto)
    {
        DemandeCredit? demande = await _context.Demandes.FindAsync(id);
        if (demande is null) return NotFound();

        bool transitionValide = TransitionsAutorisees.TryGetValue(demande.Statut, out StatutDemande[]? autorisees)
            && autorisees.Contains(dto.NouveauStatut);

        if (!transitionValide)
        {
            return BadRequest($"Transition invalide : {demande.Statut} -> {dto.NouveauStatut}.");
        }

        demande.Statut = dto.NouveauStatut;
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
