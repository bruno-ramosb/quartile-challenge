using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Quartile.Functions.Models;

namespace Quartile.Functions.Services;

public class ProductService : IProductService
{
    private readonly string _connectionString;

    public ProductService(IConfiguration configuration)
    {
        _connectionString = configuration["SqlConnectionString"] ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync(Guid? companyId = null, Guid? storeId = null)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        
        var query = "SELECT Id, Name, Sku, Price, Stock, CompanyId, StoreId, CreatedAt, UpdatedAt FROM Products WHERE 1=1";
        var parameters = new DynamicParameters();
        
        if (companyId.HasValue)
        {
            query += " AND CompanyId = @CompanyId";
            parameters.Add("@CompanyId", companyId.Value);
        }
        
        if (storeId.HasValue)
        {
            query += " AND StoreId = @StoreId";
            parameters.Add("@StoreId", storeId.Value);
        }
        
        return await connection.QueryAsync<Product>(query, parameters);
    }

    public async Task<Product?> GetProductByIdAsync(Guid id)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        
        var query = "SELECT Id, Name, Sku, Price, Stock, CompanyId, StoreId, CreatedAt, UpdatedAt FROM Products WHERE Id = @Id";
        return await connection.QueryFirstOrDefaultAsync<Product>(query, new { Id = id });
    }

    public async Task<Product> CreateProductAsync(Product product)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        
        product.Id = Guid.NewGuid();
        product.CreatedAt = DateTime.UtcNow;
        product.UpdatedAt = DateTime.UtcNow;
        
        var query = @"
            INSERT INTO Products (Id, Name, Sku, Price, Stock, CompanyId, StoreId, CreatedAt, UpdatedAt)
            VALUES (@Id, @Name, @Sku, @Price, @Stock, @CompanyId, @StoreId, @CreatedAt, @UpdatedAt)";
        
        await connection.ExecuteAsync(query, product);
        return product;
    }

    public async Task<Product?> UpdateProductAsync(Guid id, Product product)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        
        var query = @"
            UPDATE Products 
            SET Name = @Name, Sku = @Sku, Price = @Price, Stock = @Stock, 
                CompanyId = @CompanyId, StoreId = @StoreId, UpdatedAt = @UpdatedAt
            WHERE Id = @Id";
        
        var parameters = new
        {
            Id = id,
            product.Name,
            product.Sku,
            product.Price,
            product.Stock,
            product.CompanyId,
            product.StoreId,
            UpdatedAt = DateTime.UtcNow
        };
        
        var rowsAffected = await connection.ExecuteAsync(query, parameters);
        
        if (rowsAffected > 0)
        {
            product.Id = id;
            product.UpdatedAt = DateTime.UtcNow;
            return product;
        }
        
        return null;
    }

    public async Task<bool> DeleteProductAsync(Guid id)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        
        var query = "DELETE FROM Products WHERE Id = @Id";
        var rowsAffected = await connection.ExecuteAsync(query, new { Id = id });
        return rowsAffected > 0;
    }

    public async Task<string> GetProductsAsJsonAsync(Guid? companyId = null, Guid? storeId = null)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        
        var query = "SELECT dbo.GetProductsAsJson(@CompanyId, @StoreId) as JsonResult";
        var parameters = new DynamicParameters();
        parameters.Add("@CompanyId", companyId);
        parameters.Add("@StoreId", storeId);
        
        var result = await connection.QueryFirstOrDefaultAsync<string>(query, parameters);
        return result ?? "[]";
    }

    public async Task<string> GetProductsListAsync(Guid? companyId = null, Guid? storeId = null)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        
        var query = "SELECT dbo.GetProductsList(@CompanyId, @StoreId) as ProductList";
        var parameters = new DynamicParameters();
        parameters.Add("@CompanyId", companyId);
        parameters.Add("@StoreId", storeId);
        
        var result = await connection.QueryFirstOrDefaultAsync<string>(query, parameters);
        return result ?? "No products found.";
    }
} 