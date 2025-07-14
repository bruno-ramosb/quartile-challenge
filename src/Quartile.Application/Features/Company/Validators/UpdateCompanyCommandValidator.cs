using FluentValidation;
using Quartile.Application.Features.Company.Commands;
using Quartile.Domain.Enums;

namespace Quartile.Application.Features.Company.Validators
{
    public class UpdateCompanyCommandValidator : AbstractValidator<UpdateCompanyCommand>
    {
        public UpdateCompanyCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Id is required");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required")
                .MaximumLength(100)
                .WithMessage("Name cannot exceed 100 characters");

            RuleFor(x => x.DocumentNumber)
                .NotEmpty()
                .WithMessage("Document number is required")
                .MaximumLength(14)
                .WithMessage("Document number cannot exceed 14 characters");

            RuleFor(x => x.DocumentType)
                .IsInEnum()
                .WithMessage("Document type must be a valid enum value");
        }
    }
} 