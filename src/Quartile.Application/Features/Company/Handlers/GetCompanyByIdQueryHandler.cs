using MediatR;
using Quartile.Application.Common.Response;
using Quartile.Application.Dtos;
using Quartile.Application.Features.Company.Commands;
using Quartile.Domain.Interfaces.Repositories;
using System.Net;

namespace Quartile.Application.Features.Company.Handlers
{
    public class GetCompanyByIdQueryHandler : IRequestHandler<GetCompanyByIdQuery, Result<CompanyDto>>
    {
        private readonly ICompanyRepository _companyRepository;

        public GetCompanyByIdQueryHandler(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

        public async Task<Result<CompanyDto>> Handle(GetCompanyByIdQuery request, CancellationToken cancellationToken)
        {
            var company = await _companyRepository.GetByIdAsync(request.Id);
            
            if (company == null)
            {
                return Result<CompanyDto>.Fail("Company not found", HttpStatusCode.NotFound);
            }

            var companyDto = new CompanyDto(
                company.Id,
                company.Name,
                company.DocumentNumber,
                company.DocumentType,
                company.CreatedAt,
                company.UpdatedAt);

            return Result<CompanyDto>.Successful(companyDto, "Company retrieved successfully");
        }
    }
} 