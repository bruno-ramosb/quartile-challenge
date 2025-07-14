using FluentAssertions;
using MediatR;
using NSubstitute;
using Quartile.Application.Features.Company.Commands;
using Quartile.Application.Features.Company.Handlers;
using Quartile.Application.Features.Company.Responses;
using Quartile.Domain.Entities;
using Quartile.Domain.Enums;
using Quartile.Domain.Interfaces.Repositories;

namespace Quartile.Tests.Application.Features.Company.Handlers;

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
    public async Task Handle_WithValidCommand_ShouldCreateCompany()
    {
        // Arrange
        var command = new CreateCompanyCommand(
            "Test Company",
            "123456789",
            DocumentType.EIN);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.Name.Should().Be("Test Company");
        result.DocumentNumber.Should().Be("123456789");
        result.DocumentType.Should().Be(DocumentType.EIN);
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));

        await _companyRepository.Received(1).AddAsync(Arg.Any<Quartile.Domain.Entities.Company>());
        await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldSetCompanyPropertiesCorrectly()
    {
        // Arrange
        var command = new CreateCompanyCommand(
            "Another Company",
            "987654321",
            DocumentType.CNPJ);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Name.Should().Be("Another Company");
        result.DocumentNumber.Should().Be("987654321");
        result.DocumentType.Should().Be(DocumentType.CNPJ);
    }
} 