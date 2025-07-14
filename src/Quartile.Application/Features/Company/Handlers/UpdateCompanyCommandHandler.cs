using MediatR;
using Quartile.Application.Common.Response;
using Quartile.Application.Dtos;
using Quartile.Application.Features.Company.Commands;
using Quartile.Domain.Interfaces.Repositories;
using System.Net;

namespace Quartile.Application.Features.Company.Handlers
{
    public class UpdateCompanyCommandHandler : IRequestHandler<UpdateCompanyCommand, Result<CompanyDto>>
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCompanyCommandHandler(ICompanyRepository companyRepository, IUnitOfWork unitOfWork)
        {
            _companyRepository = companyRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<CompanyDto>> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
        {
            var company = await _companyRepository.GetByIdAsync(request.Id);
            
            if (company == null)
            {
                return Result<CompanyDto>.Fail("Company not found", HttpStatusCode.NotFound);
            }

            var existingCompany = await _companyRepository.GetByDocumentNumberAsync(request.DocumentNumber);
            if (existingCompany != null && existingCompany.Id != request.Id)
            {
                return Result<CompanyDto>.Fail("Another company with this document number already exists", HttpStatusCode.Conflict);
            }

            company.Name = request.Name;
            company.DocumentNumber = request.DocumentNumber;
            company.DocumentType = request.DocumentType;

            await _companyRepository.UpdateAsync(company);
            await _unitOfWork.CommitAsync(cancellationToken);

            var companyDto = new CompanyDto(
                company.Id,
                company.Name,
                company.DocumentNumber,
                company.DocumentType,
                company.CreatedAt,
                company.UpdatedAt);

            return Result<CompanyDto>.Successful(companyDto, "Company updated successfully");
        }
    }
} 