using MediatR;
using Quartile.Application.Common.Response;

namespace Quartile.Application.Features.Company.Commands
{
    public record DeleteCompanyCommand(Guid Id) : IRequest<Result<bool>>;
} 