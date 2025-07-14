using Quartile.Domain.Interfaces.Entities;

namespace Quartile.Domain.Entities;

public class Store : AuditableEntity<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    
    public Guid CompanyId { get; set; }
    public virtual Company Company { get; set; } = null!;
} 