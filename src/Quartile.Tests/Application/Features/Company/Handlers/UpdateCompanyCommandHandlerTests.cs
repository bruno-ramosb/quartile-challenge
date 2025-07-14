using FluentAssertions;
using NSubstitute;
using Quartile.Application.Common.Response;
using Quartile.Application.Dtos;
using Quartile.Application.Features.Company.Commands;
using Quartile.Application.Features.Company.Handlers;
using Quartile.Domain.Entities;
using Company = Quartile.Domain.Entities.Company;
using Quartile.Domain.Enums;
using Quartile.Domain.Interfaces.Repositories;
using System.Net;

namespace Quartile.Tests.Application.Features.Company.Handlers
{
    public class UpdateCompanyCommandHandlerTests
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UpdateCompanyCommandHandler _handler;

        public UpdateCompanyCommandHandlerTests()
        {
            _companyRepository = Substitute.For<ICompanyRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _handler = new UpdateCompanyCommandHandler(_companyRepository, _unitOfWork);
        }

        [Fact]
        public async Task Handle_WhenCompanyExistsAndNoConflict_ShouldUpdateCompanySuccessfully()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var command = new UpdateCompanyCommand(
                companyId,
                "Updated Company",
                "98765432109876",
                DocumentType.EIN);

            var existingCompany = new Quartile.Domain.Entities.Company
            {
                Id = companyId,
                Name = "Original Company",
                DocumentNumber = "12345678901234",
                DocumentType = DocumentType.CNPJ,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow.AddHours(-1)
            };

            _companyRepository.GetByIdAsync(companyId).Returns(existingCompany);
            _companyRepository.GetByDocumentNumberAsync(command.DocumentNumber).Returns((Quartile.Domain.Entities.Company?)null);
            _unitOfWork.CommitAsync(Arg.Any<CancellationToken>()).Returns(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Data.Should().NotBeNull();
            result.Data!.Id.Should().Be(companyId);
            result.Data.Name.Should().Be(command.Name);
            result.Data.DocumentNumber.Should().Be(command.DocumentNumber);
            result.Data.DocumentType.Should().Be(command.DocumentType);

            await _companyRepository.Received(1).UpdateAsync(Arg.Is<Quartile.Domain.Entities.Company>(c => 
                c.Id == companyId && 
                c.Name == command.Name && 
                c.DocumentNumber == command.DocumentNumber && 
                c.DocumentType == command.DocumentType));
            await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_WhenCompanyDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var command = new UpdateCompanyCommand(
                companyId,
                "Updated Company",
                "98765432109876",
                DocumentType.EIN);

            _companyRepository.GetByIdAsync(companyId).Returns((Quartile.Domain.Entities.Company?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Notifications.Should().Contain("Company not found");

            await _companyRepository.DidNotReceive().UpdateAsync(Arg.Any<Quartile.Domain.Entities.Company>());
            await _unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_WhenDocumentNumberConflictExists_ShouldReturnConflict()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var command = new UpdateCompanyCommand(
                companyId,
                "Updated Company",
                "98765432109876",
                DocumentType.EIN);

            var existingCompany = new Quartile.Domain.Entities.Company
            {
                Id = companyId,
                Name = "Original Company",
                DocumentNumber = "12345678901234",
                DocumentType = DocumentType.CNPJ
            };

            var conflictingCompany = new Quartile.Domain.Entities.Company
            {
                Id = Guid.NewGuid(),
                Name = "Conflicting Company",
                DocumentNumber = command.DocumentNumber,
                DocumentType = DocumentType.EIN
            };

            _companyRepository.GetByIdAsync(companyId).Returns(existingCompany);
            _companyRepository.GetByDocumentNumberAsync(command.DocumentNumber).Returns(conflictingCompany);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.Conflict);
            result.Notifications.Should().Contain("Another company with this document number already exists");

            await _companyRepository.DidNotReceive().UpdateAsync(Arg.Any<Quartile.Domain.Entities.Company>());
            await _unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
        }
    }
} 