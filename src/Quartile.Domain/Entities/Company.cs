using Quartile.Domain.Enums;
using Quartile.Domain.Interfaces.Entities;

namespace Quartile.Domain.Entities;

public class Company : AuditableEntity<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string DocumentNumber { get; set; } = string.Empty;
    public DocumentType DocumentType { get; set; }
} 