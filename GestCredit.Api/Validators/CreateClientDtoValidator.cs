using FluentValidation;
using GestCredit.Api.Dtos;

namespace GestCredit.Api.Validators;

public class CreateClientDtoValidator : AbstractValidator<CreateClientDto>
{
    public CreateClientDtoValidator()
    {
        RuleFor(x => x.Nom)
            .NotEmpty().WithMessage("Le nom est obligatoire.")
            .MaximumLength(100).WithMessage("Le nom ne peut pas dépasser 100 caractères.");

        RuleFor(x => x.Ville)
            .NotEmpty().WithMessage("La ville est obligatoire.")
            .MaximumLength(100);

        // Email est optionnel (string? dans le DTO) : on ne le valide
        // que s'il est renseigné, on ne l'exige plus avec NotEmpty().
        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Format d'email invalide.")
            .MaximumLength(150)
            .When(x => !string.IsNullOrWhiteSpace(x.Email));
    }
}
