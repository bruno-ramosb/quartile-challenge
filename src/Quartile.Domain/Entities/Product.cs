using Quartile.Domain.Interfaces.Entities;

namespace Quartile.Domain.Entities
{
    public class Product : AuditableEntity<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public Guid StoreId { get; set; }

        public virtual Store Store { get; set; } = null!;
    }
} 