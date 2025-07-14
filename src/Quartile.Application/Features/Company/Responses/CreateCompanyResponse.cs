using Quartile.Domain.Enums;

namespace Quartile.Application.Features.Company.Responses;

public record CreateCompanyResponse(
    Guid Id,
    string Name,
    string DocumentNumber,
    DocumentType DocumentType,
    DateTime CreatedAt); 