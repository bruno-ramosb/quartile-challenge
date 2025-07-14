using MediatR;
using Quartile.Application.Common.Response;
using Quartile.Application.Dtos;
using Quartile.Application.Features.Company.Commands;
using Quartile.Domain.Entities;
using Quartile.Domain.Interfaces.Repositories;
using System.Net;

namespace Quartile.Application.Features.Company.Handlers
{
    public class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, Result<CompanyDto>>
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateCompanyCommandHandler(ICompanyRepository companyRepository, IUnitOfWork unitOfWork)
        {
            _companyRepository = companyRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<CompanyDto>> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
        {
            var existingCompany = await _companyRepository.GetByDocumentNumberAsync(request.DocumentNumber);
            if (existingCompany != null)
            {
                return Result<CompanyDto>.Fail("Company with this document number already exists", HttpStatusCode.Conflict);
            }

            var company = new Domain.Entities.Company
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                DocumentNumber = request.DocumentNumber,
                DocumentType = request.DocumentType,
            };

            await _companyRepository.AddAsync(company);
            await _unitOfWork.CommitAsync(cancellationToken);

            var companyDto = new CompanyDto(
                company.Id,
                company.Name,
                company.DocumentNumber,
                company.DocumentType,
                company.CreatedAt,
                company.UpdatedAt);

            return Result<CompanyDto>.Successful(companyDto, "Company created successfully");
        }
    }
} 