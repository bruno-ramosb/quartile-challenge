using MediatR;
using Quartile.Application.Common.Response;
using Quartile.Application.Dtos;
using Quartile.Application.Features.Company.Commands;
using Quartile.Domain.Entities;
using Quartile.Domain.Interfaces.Repositories;
using System.Net;
using Microsoft.Extensions.Logging;

namespace Quartile.Application.Features.Company.Handlers
{
    public class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, Result<CompanyDto>>
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateCompanyCommandHandler> _logger;

        public CreateCompanyCommandHandler(ICompanyRepository companyRepository, IUnitOfWork unitOfWork, ILogger<CreateCompanyCommandHandler> logger)
        {
            _companyRepository = companyRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<CompanyDto>> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating company with name: {CompanyName} and document: {DocumentNumber}", request.Name, request.DocumentNumber);
            
            var existingCompany = await _companyRepository.GetByDocumentNumberAsync(request.DocumentNumber);
            if (existingCompany != null)
            {
                _logger.LogWarning("Company with document number already exists: {DocumentNumber}", request.DocumentNumber);
                return Result<CompanyDto>.Fail("Company with this document number already exists", HttpStatusCode.Conflict);
            }

            var company = new Domain.Entities.Company
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                DocumentNumber = request.DocumentNumber,
                DocumentType = request.DocumentType,
            };

            try
            {
                await _companyRepository.AddAsync(company);
                await _unitOfWork.CommitAsync(cancellationToken);
                
                var companyDto = new CompanyDto(
                    company.Id,
                    company.Name,
                    company.DocumentNumber,
                    company.DocumentType,
                    company.CreatedAt,
                    company.UpdatedAt);

                _logger.LogInformation("Company created successfully with ID: {CompanyId}", company.Id);
                return Result<CompanyDto>.Successful(companyDto, "Company created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating company with name: {CompanyName}", request.Name);
                throw;
            }
        }
    }
} 