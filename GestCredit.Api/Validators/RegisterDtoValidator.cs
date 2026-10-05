using FluentValidation;
using GestCredit.Api.Dtos;

namespace GestCredit.Api.Validators;

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(x => x.NomUtilisateur)
            .NotEmpty().WithMessage("Le nom d'utilisateur est obligatoire.")
            .MaximumLength(100);

        RuleFor(x => x.MotDePasse)
            .NotEmpty().WithMessage("Le mot de passe est obligatoire.")
            .MinimumLength(8).WithMessage("Le mot de passe doit contenir au moins 8 caractères.");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Le rôle est obligatoire.")
            .Must(r => r is "Agent" or "Analyste" or "Administrateur")
            .WithMessage("Le rôle doit être Agent, Analyste ou Administrateur.");
    }
}
