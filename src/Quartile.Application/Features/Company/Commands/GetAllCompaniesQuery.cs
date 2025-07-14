using MediatR;
using Quartile.Application.Common.Response;
using Quartile.Application.Dtos;

namespace Quartile.Application.Features.Company.Commands
{
    public record GetAllCompaniesQuery() : IRequest<Result<IEnumerable<CompanyDto>>>;
} 