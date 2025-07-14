using FluentAssertions;
using Quartile.Domain.Entities;
using Quartile.Domain.Enums;

namespace Quartile.Tests.Domain.Entities;

public class CompanyTests
{
    [Fact]
    public void Company_ShouldCreateWithValidProperties()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var company = new Company
        {
            Id = companyId,
            Name = "Test Company",
            DocumentNumber = "123456789",
            DocumentType = DocumentType.EIN,
            CreatedAt = DateTime.UtcNow
        };

        // Assert
        company.Id.Should().Be(companyId);
        company.Name.Should().Be("Test Company");
        company.DocumentNumber.Should().Be("123456789");
        company.DocumentType.Should().Be(DocumentType.EIN);
    }

    [Fact]
    public void Company_ShouldHaveDefaultValues()
    {
        // Arrange
        var company = new Company
        {
            DocumentType = DocumentType.EIN
        };

        // Assert
        company.Id.Should().BeEmpty();
        company.Name.Should().BeEmpty();
        company.DocumentNumber.Should().BeEmpty();
        company.DocumentType.Should().Be(DocumentType.EIN);
    }
} 