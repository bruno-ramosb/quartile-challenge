using System.Net;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Quartile.Application.Features.Company.Commands;
using Quartile.Application.Features.Company.Handlers;
using Quartile.Domain.Entities;
using Company = Quartile.Domain.Entities.Company;
using Quartile.Domain.Enums;
using Quartile.Domain.Interfaces.Repositories;

namespace Quartile.Tests.Application.Features.Company.Handlers
{
    public class DeleteCompanyCommandHandlerTests
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly DeleteCompanyCommandHandler _handler;

        public DeleteCompanyCommandHandlerTests()
        {
            _companyRepository = Substitute.For<ICompanyRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _handler = new DeleteCompanyCommandHandler(_companyRepository, _unitOfWork);
        }

        [Fact]
        public async Task Handle_WhenCompanyExists_ShouldDeleteCompanySuccessfully()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var command = new DeleteCompanyCommand(companyId);

            var company = new Quartile.Domain.Entities.Company
            {
                Id = companyId,
                Name = "Test Company",
                DocumentNumber = "12345678901234",
                DocumentType = DocumentType.CNPJ
            };

            _companyRepository.GetByIdAsync(companyId).Returns(company);
            _unitOfWork.CommitAsync(Arg.Any<CancellationToken>()).Returns(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Data.Should().BeTrue();

            await _companyRepository.Received(1).RemoveAsync(Arg.Is<Quartile.Domain.Entities.Company>(c => c.Id == companyId));
            await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_WhenCompanyDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var command = new DeleteCompanyCommand(companyId);

            _companyRepository.GetByIdAsync(companyId).Returns((Quartile.Domain.Entities.Company?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Notifications.Should().Contain("Company not found");

            await _companyRepository.DidNotReceive().RemoveAsync(Arg.Any<Quartile.Domain.Entities.Company>());
            await _unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_WhenUnitOfWorkFails_ShouldThrowException()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var command = new DeleteCompanyCommand(companyId);

            var company = new Quartile.Domain.Entities.Company
            {
                Id = companyId,
                Name = "Test Company",
                DocumentNumber = "12345678901234",
                DocumentType = DocumentType.CNPJ
            };

            _companyRepository.GetByIdAsync(companyId).Returns(company);
            _unitOfWork.CommitAsync(Arg.Any<CancellationToken>()).Throws(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        }
    }
} 