using FluentAssertions;
using MediatR;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
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
    public class CreateCompanyCommandHandlerTests
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly CreateCompanyCommandHandler _handler;

        public CreateCompanyCommandHandlerTests()
        {
            _companyRepository = Substitute.For<ICompanyRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _handler = new CreateCompanyCommandHandler(_companyRepository, _unitOfWork);
        }

        [Fact]
        public async Task Handle_WhenCompanyDoesNotExist_ShouldCreateCompanySuccessfully()
        {
            // Arrange
            var command = new CreateCompanyCommand(
                "Test Company",
                "12345678901234",
                DocumentType.CNPJ);

            _companyRepository.GetByDocumentNumberAsync(command.DocumentNumber).Returns((Company?)null);
            _unitOfWork.CommitAsync(Arg.Any<CancellationToken>()).Returns(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Data.Should().NotBeNull();
            result.Data!.Name.Should().Be(command.Name);
            result.Data.DocumentNumber.Should().Be(command.DocumentNumber);
            result.Data.DocumentType.Should().Be(command.DocumentType);

            await _companyRepository.Received(1).AddAsync(Arg.Any<Company>());
            await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_WhenCompanyWithSameDocumentNumberExists_ShouldReturnConflict()
        {
            // Arrange
            var command = new CreateCompanyCommand(
                "Test Company",
                "12345678901234",
                DocumentType.CNPJ);

            var existingCompany = new Company
            {
                Id = Guid.NewGuid(),
                Name = "Existing Company",
                DocumentNumber = command.DocumentNumber,
                DocumentType = DocumentType.CNPJ
            };

            _companyRepository.GetByDocumentNumberAsync(command.DocumentNumber).Returns(existingCompany);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.Conflict);
            result.Notifications.Should().Contain("Company with this document number already exists");

            await _companyRepository.DidNotReceive().AddAsync(Arg.Any<Company>());
            await _unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_WhenUnitOfWorkFails_ShouldThrowException()
        {
            // Arrange
            var command = new CreateCompanyCommand(
                "Test Company",
                "12345678901234",
                DocumentType.CNPJ);

            _companyRepository.GetByDocumentNumberAsync(command.DocumentNumber).Returns((Company?)null);
            _unitOfWork.CommitAsync(Arg.Any<CancellationToken>()).Throws(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        }
    }
} 