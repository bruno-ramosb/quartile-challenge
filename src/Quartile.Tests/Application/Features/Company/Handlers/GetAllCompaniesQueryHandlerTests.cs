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
    public class GetAllCompaniesQueryHandlerTests
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly GetAllCompaniesQueryHandler _handler;

        public GetAllCompaniesQueryHandlerTests()
        {
            _companyRepository = Substitute.For<ICompanyRepository>();
            _handler = new GetAllCompaniesQueryHandler(_companyRepository);
        }

        [Fact]
        public async Task Handle_WhenCompaniesExist_ShouldReturnAllCompaniesSuccessfully()
        {
            // Arrange
            var query = new GetAllCompaniesQuery();

            var companies = new List<Quartile.Domain.Entities.Company>
            {
                new Quartile.Domain.Entities.Company
                {
                    Id = Guid.NewGuid(),
                    Name = "Company A",
                    DocumentNumber = "12345678901234",
                    DocumentType = DocumentType.CNPJ,
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    UpdatedAt = DateTime.UtcNow.AddDays(-1)
                },
                new Quartile.Domain.Entities.Company
                {
                    Id = Guid.NewGuid(),
                    Name = "Company B",
                    DocumentNumber = "98765432109876",
                    DocumentType = DocumentType.EIN,
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    UpdatedAt = DateTime.UtcNow
                }
            };

            _companyRepository.GetAllAsync().Returns(companies);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Data.Should().NotBeNull();
            result.Data!.Should().HaveCount(2);

            var companyDtos = result.Data!.ToList();
            companyDtos[0].Name.Should().Be("Company A");
            companyDtos[0].DocumentNumber.Should().Be("12345678901234");
            companyDtos[0].DocumentType.Should().Be(DocumentType.CNPJ);
            companyDtos[1].Name.Should().Be("Company B");
            companyDtos[1].DocumentNumber.Should().Be("98765432109876");
            companyDtos[1].DocumentType.Should().Be(DocumentType.EIN);

            await _companyRepository.Received(1).GetAllAsync();
        }

        [Fact]
        public async Task Handle_WhenNoCompaniesExist_ShouldReturnEmptyListSuccessfully()
        {
            // Arrange
            var query = new GetAllCompaniesQuery();

            _companyRepository.GetAllAsync().Returns(new List<Quartile.Domain.Entities.Company>());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Data.Should().NotBeNull();
            result.Data!.Should().BeEmpty();

            await _companyRepository.Received(1).GetAllAsync();
        }
    }
} 