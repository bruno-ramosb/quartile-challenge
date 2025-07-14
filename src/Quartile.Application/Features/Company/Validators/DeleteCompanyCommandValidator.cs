using FluentValidation;
using Quartile.Application.Features.Company.Commands;

namespace Quartile.Application.Features.Company.Validators
{
    public class DeleteCompanyCommandValidator : AbstractValidator<DeleteCompanyCommand>
    {
        public DeleteCompanyCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Id is required");
        }
    }
} 