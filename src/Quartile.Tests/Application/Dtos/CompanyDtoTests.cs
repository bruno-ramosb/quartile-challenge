using FluentAssertions;
using Quartile.Application.Dtos;
using Quartile.Domain.Enums;

namespace Quartile.Tests.Application.Dtos
{
    public class CompanyDtoTests
    {
        [Fact]
        public void CompanyDto_WhenCreated_ShouldHaveCorrectProperties()
        {
            // Arrange
            var id = Guid.NewGuid();
            var name = "Test Company";
            var documentNumber = "12345678901234";
            var documentType = DocumentType.CNPJ;
            var createdAt = DateTime.UtcNow.AddDays(-1);
            var updatedAt = DateTime.UtcNow;

            // Act
            var companyDto = new CompanyDto(id, name, documentNumber, documentType, createdAt, updatedAt);

            // Assert
            companyDto.Id.Should().Be(id);
            companyDto.Name.Should().Be(name);
            companyDto.DocumentNumber.Should().Be(documentNumber);
            companyDto.DocumentType.Should().Be(documentType);
            companyDto.CreatedAt.Should().Be(createdAt);
            companyDto.UpdatedAt.Should().Be(updatedAt);
        }

        [Fact]
        public void CompanyDto_WhenCreatedWithDifferentValues_ShouldHaveCorrectProperties()
        {
            // Arrange
            var id = Guid.NewGuid();
            var name = "Another Company";
            var documentNumber = "98765432109876";
            var documentType = DocumentType.EIN;
            var createdAt = DateTime.UtcNow.AddDays(-5);
            var updatedAt = DateTime.UtcNow.AddDays(-1);

            // Act
            var companyDto = new CompanyDto(id, name, documentNumber, documentType, createdAt, updatedAt);

            // Assert
            companyDto.Id.Should().Be(id);
            companyDto.Name.Should().Be(name);
            companyDto.DocumentNumber.Should().Be(documentNumber);
            companyDto.DocumentType.Should().Be(documentType);
            companyDto.CreatedAt.Should().Be(createdAt);
            companyDto.UpdatedAt.Should().Be(updatedAt);
        }

        [Fact]
        public void CompanyDto_WhenCreatedWithSSN_ShouldHaveCorrectDocumentType()
        {
            // Arrange
            var id = Guid.NewGuid();
            var name = "SSN Company";
            var documentNumber = "123456789";
            var documentType = DocumentType.SSN;
            var createdAt = DateTime.UtcNow.AddDays(-1);
            var updatedAt = DateTime.UtcNow;

            // Act
            var companyDto = new CompanyDto(id, name, documentNumber, documentType, createdAt, updatedAt);

            // Assert
            companyDto.DocumentType.Should().Be(DocumentType.SSN);
        }
    }
} 