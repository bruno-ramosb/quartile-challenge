using Quartile.Functions.Models;

namespace Quartile.Functions.Services;

public interface IProductService
{
    Task<IEnumerable<Product>> GetAllProductsAsync(Guid? storeId = null);
    Task<Product?> GetProductByIdAsync(Guid id);
    Task<Product> CreateProductAsync(Product product);
    Task<Product?> UpdateProductAsync(Guid id, Product product);
    Task<bool> DeleteProductAsync(Guid id);
    Task<string> GetProductsAsJsonAsync(Guid? storeId = null);
    Task<string> GetProductsListAsync(Guid? storeId = null);
} 