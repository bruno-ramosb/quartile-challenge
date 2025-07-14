using FluentAssertions;
using Quartile.Application.Features.Company.Commands;
using Quartile.Application.Features.Company.Validators;
using Quartile.Domain.Enums;

namespace Quartile.Tests.Application.Features.Company.Validators
{
    public class CreateCompanyCommandValidatorTests
    {
        private readonly CreateCompanyCommandValidator _validator;

        public CreateCompanyCommandValidatorTests()
        {
            _validator = new CreateCompanyCommandValidator();
        }

        [Fact]
        public void Validate_WhenCommandIsValid_ShouldNotHaveValidationErrors()
        {
            // Arrange
            var command = new CreateCompanyCommand(
                "Valid Company Name",
                "12345678901234",
                DocumentType.CNPJ);

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void Validate_WhenNameIsEmpty_ShouldHaveValidationError(string name)
        {
            // Arrange
            var command = new CreateCompanyCommand(
                name,
                "12345678901234",
                DocumentType.CNPJ);

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(command.Name));
            result.Errors.Should().ContainSingle(e => e.ErrorMessage == "Name is required");
        }

        [Fact]
        public void Validate_WhenNameExceedsMaxLength_ShouldHaveValidationError()
        {
            // Arrange
            var longName = new string('A', 101);
            var command = new CreateCompanyCommand(
                longName,
                "12345678901234",
                DocumentType.CNPJ);

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(command.Name));
            result.Errors.Should().ContainSingle(e => e.ErrorMessage == "Name cannot exceed 100 characters");
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void Validate_WhenDocumentNumberIsEmpty_ShouldHaveValidationError(string documentNumber)
        {
            // Arrange
            var command = new CreateCompanyCommand(
                "Valid Company Name",
                documentNumber,
                DocumentType.CNPJ);

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(command.DocumentNumber));
            result.Errors.Should().ContainSingle(e => e.ErrorMessage == "Document number is required");
        }

        [Fact]
        public void Validate_WhenDocumentNumberExceedsMaxLength_ShouldHaveValidationError()
        {
            // Arrange
            var longDocumentNumber = new string('1', 15);
            var command = new CreateCompanyCommand(
                "Valid Company Name",
                longDocumentNumber,
                DocumentType.CNPJ);

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(command.DocumentNumber));
            result.Errors.Should().ContainSingle(e => e.ErrorMessage == "Document number cannot exceed 14 characters");
        }

        [Fact]
        public void Validate_WhenDocumentTypeIsInvalid_ShouldHaveValidationError()
        {
            // Arrange
            var command = new CreateCompanyCommand(
                "Valid Company Name",
                "12345678901234",
                (DocumentType)999); // Invalid enum value

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(command.DocumentType));
            result.Errors.Should().ContainSingle(e => e.ErrorMessage == "Document type must be a valid enum value");
        }

        [Theory]
        [InlineData(DocumentType.EIN)]
        [InlineData(DocumentType.SSN)]
        [InlineData(DocumentType.CNPJ)]
        public void Validate_WhenDocumentTypeIsValid_ShouldNotHaveValidationError(DocumentType documentType)
        {
            // Arrange
            var command = new CreateCompanyCommand(
                "Valid Company Name",
                "12345678901234",
                documentType);

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().NotContain(e => e.PropertyName == nameof(command.DocumentType));
        }
    }
} 