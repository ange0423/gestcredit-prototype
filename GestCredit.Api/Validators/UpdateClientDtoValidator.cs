using FluentValidation;
using GestCredit.Api.Dtos;

namespace GestCredit.Api.Validators;

public class UpdateClientDtoValidator : AbstractValidator<UpdateClientDto>
{
    public UpdateClientDtoValidator()
    {
        RuleFor(x => x.Nom)
            .NotEmpty().WithMessage("Le nom est obligatoire.")
            .MaximumLength(100);

        RuleFor(x => x.Ville)
            .NotEmpty().WithMessage("La ville est obligatoire.")
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Format d'email invalide.")
            .MaximumLength(150)
            .When(x => !string.IsNullOrWhiteSpace(x.Email));
    }
}
