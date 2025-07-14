using FluentAssertions;
using FluentValidation.TestHelper;
using Quartile.Application.Features.Company.Commands;
using Quartile.Application.Features.Company.Validators;
using Quartile.Domain.Enums;

namespace Quartile.Tests.Application.Features.Company.Validators;

public class CreateCompanyCommandValidatorTests
{
    private readonly CreateCompanyCommandValidator _validator;

    public CreateCompanyCommandValidatorTests()
    {
        _validator = new CreateCompanyCommandValidator();
    }

    [Fact]
    public void Validate_WithValidData_ShouldPass()
    {
        // Arrange
        var command = new CreateCompanyCommand(
            "Test Company",
            "123456789",
            DocumentType.EIN);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Validate_WithInvalidName_ShouldFail(string name)
    {
        // Arrange
        var command = new CreateCompanyCommand(
            name,
            "123456789",
            DocumentType.EIN);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Validate_WithInvalidDocumentNumber_ShouldFail(string documentNumber)
    {
        // Arrange
        var command = new CreateCompanyCommand(
            "Test Company",
            documentNumber,
            DocumentType.EIN);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DocumentNumber);
    }

    [Theory]
    [InlineData((DocumentType)999)] // Invalid enum value
    public void Validate_WithInvalidDocumentType_ShouldFail(DocumentType documentType)
    {
        // Arrange
        var command = new CreateCompanyCommand(
            "Test Company",
            "123456789",
            documentType);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DocumentType);
    }

    [Theory]
    [InlineData(DocumentType.EIN)]
    [InlineData(DocumentType.SSN)]
    [InlineData(DocumentType.CNPJ)]
    public void Validate_WithValidDocumentType_ShouldPass(DocumentType documentType)
    {
        // Arrange
        var command = new CreateCompanyCommand(
            "Test Company",
            "123456789",
            documentType);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.DocumentType);
    }
} 