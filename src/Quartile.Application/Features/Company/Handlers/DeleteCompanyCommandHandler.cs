using MediatR;
using Quartile.Application.Common.Response;
using Quartile.Application.Features.Company.Commands;
using Quartile.Domain.Interfaces.Repositories;
using System.Net;

namespace Quartile.Application.Features.Company.Handlers
{
    public class DeleteCompanyCommandHandler : IRequestHandler<DeleteCompanyCommand, Result<bool>>
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCompanyCommandHandler(ICompanyRepository companyRepository, IUnitOfWork unitOfWork)
        {
            _companyRepository = companyRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(DeleteCompanyCommand request, CancellationToken cancellationToken)
        {
            var company = await _companyRepository.GetByIdAsync(request.Id);
            
            if (company == null)
            {
                return Result<bool>.Fail("Company not found", HttpStatusCode.NotFound);
            }

            await _companyRepository.RemoveAsync(company);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Result<bool>.Successful(true, "Company deleted successfully");
        }
    }
} 