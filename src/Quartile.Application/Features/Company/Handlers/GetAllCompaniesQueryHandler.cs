using MediatR;
using Quartile.Application.Common.Response;
using Quartile.Application.Dtos;
using Quartile.Application.Features.Company.Commands;
using Quartile.Domain.Interfaces.Repositories;

namespace Quartile.Application.Features.Company.Handlers
{
    public class GetAllCompaniesQueryHandler : IRequestHandler<GetAllCompaniesQuery, Result<IEnumerable<CompanyDto>>>
    {
        private readonly ICompanyRepository _companyRepository;

        public GetAllCompaniesQueryHandler(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

        public async Task<Result<IEnumerable<CompanyDto>>> Handle(GetAllCompaniesQuery request, CancellationToken cancellationToken)
        {
            var companies = await _companyRepository.GetAllAsync();
            
            var companyDtos = companies.Select(company => new CompanyDto(
                company.Id,
                company.Name,
                company.DocumentNumber,
                company.DocumentType,
                company.CreatedAt,
                company.UpdatedAt));

            return Result<IEnumerable<CompanyDto>>.Successful(companyDtos, "Companies retrieved successfully");
        }
    }
} 