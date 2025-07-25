using MediatR;
using Quartile.Application.Common.Response;
using Quartile.Application.Dtos;
using Quartile.Domain.Enums;

namespace Quartile.Application.Features.Company.Commands
{
    public record CreateCompanyCommand(
        string Name,
        string DocumentNumber,
        DocumentType DocumentType) : IRequest<Result<CompanyDto>>;
} 