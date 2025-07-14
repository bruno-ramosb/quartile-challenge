using MediatR;
using Quartile.Application.Features.Company.Commands;
using Quartile.Application.Features.Company.Responses;
using Quartile.Domain.Entities;
using Quartile.Domain.Interfaces.Repositories;

namespace Quartile.Application.Features.Company.Handlers;

public class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, CreateCompanyResponse>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCompanyCommandHandler(ICompanyRepository companyRepository, IUnitOfWork unitOfWork)
    {
        _companyRepository = companyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateCompanyResponse> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = new Domain.Entities.Company
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            DocumentNumber = request.DocumentNumber,
            DocumentType = request.DocumentType,
            CreatedAt = DateTime.UtcNow
        };

        await _companyRepository.AddAsync(company);
        await _unitOfWork.CommitAsync(cancellationToken);

        return new CreateCompanyResponse(
            company.Id,
            company.Name,
            company.DocumentNumber,
            company.DocumentType,
            company.CreatedAt);
    }
} 