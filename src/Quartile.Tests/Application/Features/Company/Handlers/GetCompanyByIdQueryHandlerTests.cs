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
    public class GetCompanyByIdQueryHandlerTests
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly GetCompanyByIdQueryHandler _handler;

        public GetCompanyByIdQueryHandlerTests()
        {
            _companyRepository = Substitute.For<ICompanyRepository>();
            _handler = new GetCompanyByIdQueryHandler(_companyRepository);
        }

        [Fact]
        public async Task Handle_WhenCompanyExists_ShouldReturnCompanySuccessfully()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var query = new GetCompanyByIdQuery(companyId);

            var company = new Quartile.Domain.Entities.Company
            {
                Id = companyId,
                Name = "Test Company",
                DocumentNumber = "12345678901234",
                DocumentType = DocumentType.CNPJ,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow
            };

            _companyRepository.GetByIdAsync(companyId).Returns(company);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Data.Should().NotBeNull();
            result.Data!.Id.Should().Be(companyId);
            result.Data.Name.Should().Be(company.Name);
            result.Data.DocumentNumber.Should().Be(company.DocumentNumber);
            result.Data.DocumentType.Should().Be(company.DocumentType);
            result.Data.CreatedAt.Should().Be(company.CreatedAt);
            result.Data.UpdatedAt.Should().Be(company.UpdatedAt);

            await _companyRepository.Received(1).GetByIdAsync(companyId);
        }

        [Fact]
        public async Task Handle_WhenCompanyDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var query = new GetCompanyByIdQuery(companyId);

            _companyRepository.GetByIdAsync(companyId).Returns((Quartile.Domain.Entities.Company?)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Notifications.Should().Contain("Company not found");

            await _companyRepository.Received(1).GetByIdAsync(companyId);
        }
    }
} 