using FluentValidation;
using Quartile.Application.Features.Company.Commands;
using Quartile.Domain.Enums;

namespace Quartile.Application.Features.Company.Validators;

public class CreateCompanyCommandValidator : AbstractValidator<CreateCompanyCommand>
{
    public CreateCompanyCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.DocumentNumber)
            .NotEmpty()
            .MaximumLength(14);

        RuleFor(x => x.DocumentType)
            .IsInEnum()
            .WithMessage("DocumentType must be a valid enum value");
    }
} 