using Quartile.Domain.Enums;

namespace Quartile.Application.Dtos
{
    public record CompanyDto(
        Guid Id,
        string Name,
        string DocumentNumber,
        DocumentType DocumentType,
        DateTime CreatedAt,
        DateTime UpdatedAt);
} 