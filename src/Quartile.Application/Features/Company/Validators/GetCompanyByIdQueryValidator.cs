using FluentValidation;
using Quartile.Application.Features.Company.Commands;

namespace Quartile.Application.Features.Company.Validators
{
    public class GetCompanyByIdQueryValidator : AbstractValidator<GetCompanyByIdQuery>
    {
        public GetCompanyByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Id is required");
        }
    }
} 