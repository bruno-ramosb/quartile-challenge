using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Quartile.Functions.Models;
using Quartile.Functions.Services;

namespace Quartile.Functions.Functions;

public class ProductFunctions
{
    private readonly ILogger<ProductFunctions> _logger;
    private readonly IProductService _productService;

    public ProductFunctions(ILogger<ProductFunctions> logger, IProductService productService)
    {
        _logger = logger;
        _productService = productService;
    }

    [Function("GetProducts")]
    public async Task<HttpResponseData> GetProducts(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "products")] HttpRequestData req)
    {
        _logger.LogInformation("GetProducts function processed a request.");

        var storeId = req.Query["storeId"];
        Guid? storeIdValue = null;

        if (!string.IsNullOrEmpty(storeId) && Guid.TryParse(storeId, out var tempStoreId))
            storeIdValue = tempStoreId;

        var products = await _productService.GetAllProductsAsync(storeIdValue);
        
        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json; charset=utf-8");
        
        var jsonResponse = JsonSerializer.Serialize(products, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        await response.WriteStringAsync(jsonResponse);
        
        return response;
    }

    [Function("GetProductsAsJson")]
    public async Task<HttpResponseData> GetProductsAsJson(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "products-json")] HttpRequestData req)
    {
        _logger.LogInformation("GetProductsAsJson function processed a request.");

        var storeId = req.Query["storeId"];
        Guid? storeIdValue = null;

        if (!string.IsNullOrEmpty(storeId) && Guid.TryParse(storeId, out var tempStoreId))
            storeIdValue = tempStoreId;

        var jsonResult = await _productService.GetProductsAsJsonAsync(storeIdValue);
        
        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json; charset=utf-8");
        await response.WriteStringAsync(jsonResult);
        
        return response;
    }

    [Function("GetProductsList")]
    public async Task<HttpResponseData> GetProductsList(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "products-list")] HttpRequestData req)
    {
        _logger.LogInformation("GetProductsList function processed a request.");

        var storeId = req.Query["storeId"];
        Guid? storeIdValue = null;

        if (!string.IsNullOrEmpty(storeId) && Guid.TryParse(storeId, out var tempStoreId))
            storeIdValue = tempStoreId;

        var listResult = await _productService.GetProductsListAsync(storeIdValue);
        
        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
        await response.WriteStringAsync(listResult);
        
        return response;
    }

    [Function("GetProduct")]
    public async Task<HttpResponseData> GetProduct(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "products/{id}")] HttpRequestData req,
        string id)
    {
        _logger.LogInformation("GetProduct function processed a request for ID: {Id}", id);

        if (!Guid.TryParse(id, out var productId))
        {
            var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badRequestResponse.WriteStringAsync("Invalid product ID format");
            return badRequestResponse;
        }

        var product = await _productService.GetProductByIdAsync(productId);
        
        if (product == null)
        {
            var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
            await notFoundResponse.WriteStringAsync("Product not found");
            return notFoundResponse;
        }
        
        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json; charset=utf-8");
        
        var jsonResponse = JsonSerializer.Serialize(product, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        await response.WriteStringAsync(jsonResponse);
        
        return response;
    }

    [Function("CreateProduct")]
    public async Task<HttpResponseData> CreateProduct(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "products")] HttpRequestData req)
    {
        _logger.LogInformation("CreateProduct function processed a request.");

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        
        if (string.IsNullOrEmpty(requestBody))
        {
            var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badRequestResponse.WriteStringAsync("Request body is required");
            return badRequestResponse;
        }

        try
        {
            var product = JsonSerializer.Deserialize<Product>(requestBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            if (product == null)
            {
                var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badRequestResponse.WriteStringAsync("Invalid product data");
                return badRequestResponse;
            }

            var createdProduct = await _productService.CreateProductAsync(product);
            
            var response = req.CreateResponse(HttpStatusCode.Created);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            
            var jsonResponse = JsonSerializer.Serialize(createdProduct, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            await response.WriteStringAsync(jsonResponse);
            
            return response;
        }
        catch (JsonException)
        {
            var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badRequestResponse.WriteStringAsync("Invalid JSON format");
            return badRequestResponse;
        }
    }

    [Function("UpdateProduct")]
    public async Task<HttpResponseData> UpdateProduct(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "products/{id}")] HttpRequestData req,
        string id)
    {
        _logger.LogInformation("UpdateProduct function processed a request for ID: {Id}", id);

        if (!Guid.TryParse(id, out var productId))
        {
            var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badRequestResponse.WriteStringAsync("Invalid product ID format");
            return badRequestResponse;
        }

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        
        if (string.IsNullOrEmpty(requestBody))
        {
            var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badRequestResponse.WriteStringAsync("Request body is required");
            return badRequestResponse;
        }

        try
        {
            var product = JsonSerializer.Deserialize<Product>(requestBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            if (product == null)
            {
                var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badRequestResponse.WriteStringAsync("Invalid product data");
                return badRequestResponse;
            }

            var updatedProduct = await _productService.UpdateProductAsync(productId, product);
            
            if (updatedProduct == null)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
                await notFoundResponse.WriteStringAsync("Product not found");
                return notFoundResponse;
            }
            
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            
            var jsonResponse = JsonSerializer.Serialize(updatedProduct, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            await response.WriteStringAsync(jsonResponse);
            
            return response;
        }
        catch (JsonException)
        {
            var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badRequestResponse.WriteStringAsync("Invalid JSON format");
            return badRequestResponse;
        }
    }

    [Function("DeleteProduct")]
    public async Task<HttpResponseData> DeleteProduct(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "products/{id}")] HttpRequestData req,
        string id)
    {
        _logger.LogInformation("DeleteProduct function processed a request for ID: {Id}", id);

        if (!Guid.TryParse(id, out var productId))
        {
            var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badRequestResponse.WriteStringAsync("Invalid product ID format");
            return badRequestResponse;
        }

        var success = await _productService.DeleteProductAsync(productId);
        
        if (!success)
        {
            var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
            await notFoundResponse.WriteStringAsync("Product not found");
            return notFoundResponse;
        }
        
        var response = req.CreateResponse(HttpStatusCode.NoContent);
        return response;
    }
} 