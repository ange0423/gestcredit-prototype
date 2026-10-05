using FluentValidation;
using GestCredit.Api.Dtos;

namespace GestCredit.Api.Validators;

public class CreateDemandeCreditDtoValidator : AbstractValidator<CreateDemandeCreditDto>
{
    public CreateDemandeCreditDtoValidator()
    {
        RuleFor(x => x.ClientId)
            .GreaterThan(0).WithMessage("L'identifiant du client est obligatoire.");

        RuleFor(x => x.Montant)
            .GreaterThan(0).WithMessage("Le montant doit être strictement positif.");

        RuleFor(x => x.TauxAnnuel)
            .GreaterThan(0).WithMessage("Le taux annuel doit être strictement positif.");

        // Reprend la contrainte CHECK du Jour 12 (SQL) : DureeMois BETWEEN 1 AND 360
        RuleFor(x => x.DureeMois)
            .InclusiveBetween(1, 360).WithMessage("La durée doit être comprise entre 1 et 360 mois.");
    }
}
