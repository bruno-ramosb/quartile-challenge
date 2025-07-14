using MediatR;
using Quartile.Application.Features.Company.Responses;
using Quartile.Domain.Enums;

namespace Quartile.Application.Features.Company.Commands
{
    public record CreateCompanyCommand(
        string Name,
        string DocumentNumber,
        DocumentType DocumentType) : IRequest<CreateCompanyResponse>;
} 