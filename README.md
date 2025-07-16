# Quartile API Documentation

## Overview

Quartile API is a RESTful service built with .NET 8, implementing a hybrid architecture with traditional Web API controllers and Azure Functions. The system manages Companies, Stores, and Products with proper data relationships and validation.

## Architecture

- **Companies & Stores**: Traditional ASP.NET Core Web API with CQRS pattern (for Companies)
- **Products**: Azure Functions with serverless architecture
- **Database**: SQL Server with Entity Framework Core and Dapper
- **Validation**: FluentValidation for request validation

## Base URLs

- **Web API**: `https://localhost:7243`
- **Azure Functions**: `http://localhost:7069`

## Authentication

Currently, no authentication is required. All endpoints are publicly accessible.

## Response Format

All endpoints return JSON responses with the following structure:

```json
{
  "success": true,
  "data": { ... },
  "message": "Operation completed successfully"
}
```

## Companies API

### Base Path: `/api/v1/Company`

**Note**: Companies API implements CQRS pattern for demonstration purposes, showing command/query separation for developers who prefer this architectural approach.

#### Create Company
- **Method**: `POST`
- **URL**: `/api/v1/Company`
- **Body**:
```json
{
  "name": "Company Name",
  "documentNumber": "12345678901",
  "documentType": 1
}
```
- **Response**: `201 Created`

#### Get All Companies
- **Method**: `GET`
- **URL**: `/api/v1/Company`
- **Response**: `200 OK`

#### Get Company by ID
- **Method**: `GET`
- **URL**: `/api/v1/Company/{id}`
- **Response**: `200 OK`

#### Update Company
- **Method**: `PUT`
- **URL**: `/api/v1/Company/{id}`
- **Body**:
```json
{
  "id": "guid",
  "name": "Updated Company Name",
  "documentNumber": "98765432109",
  "documentType": 1
}
```
- **Response**: `200 OK`

#### Delete Company
- **Method**: `DELETE`
- **URL**: `/api/v1/Company/{id}`
- **Response**: `204 No Content`

## Stores API

### Base Path: `/api/Store`

#### Create Store
- **Method**: `POST`
- **URL**: `/api/Store`
- **Body**:
```json
{
  "name": "Store Name",
  "email": "store@company.com",
  "phone": "+1-555-0123",
  "address": "123 Main Street",
  "city": "New York",
  "state": "NY",
  "zipCode": "10001",
  "companyId": "company-guid"
}
```
- **Response**: `201 Created`

#### Get All Stores
- **Method**: `GET`
- **URL**: `/api/Store`
- **Response**: `200 OK`

#### Get Store by ID
- **Method**: `GET`
- **URL**: `/api/Store/{id}`
- **Response**: `200 OK`

#### Update Store
- **Method**: `PUT`
- **URL**: `/api/Store/{id}`
- **Body**: Same as Create Store
- **Response**: `200 OK`

#### Delete Store
- **Method**: `DELETE`
- **URL**: `/api/Store/{id}`
- **Response**: `204 No Content`

## Products API (Azure Functions)

### Base Path: `/api/products`

#### Create Product
- **Method**: `POST`
- **URL**: `/api/products`
- **Body**:
```json
{
  "name": "Product Name",
  "sku": "PRODUCT-SKU-001",
  "price": 99.99,
  "stock": 100,
  "storeId": "store-guid"
}
```
- **Response**: `201 Created`

#### Get All Products
- **Method**: `GET`
- **URL**: `/api/products`
- **Response**: `200 OK`

#### Get Products by Store
- **Method**: `GET`
- **URL**: `/api/products?storeId={storeId}`
- **Response**: `200 OK`

#### Get Product by ID
- **Method**: `GET`
- **URL**: `/api/products/{id}`
- **Response**: `200 OK`

#### Update Product
- **Method**: `PUT`
- **URL**: `/api/products/{id}`
- **Body**: Same as Create Product
- **Response**: `200 OK`

#### Delete Product
- **Method**: `DELETE`
- **URL**: `/api/products/{id}`
- **Response**: `204 No Content`

### Special Endpoints

#### Get Products as JSON
- **Method**: `GET`
- **URL**: `/api/products-json?storeId={storeId}`
- **Description**: Returns products in JSON format using SQL stored procedure
- **Response**: `200 OK`

#### Get Products List
- **Method**: `GET`
- **URL**: `/api/products-list?storeId={storeId}`
- **Description**: Returns products as formatted text list using SQL stored procedure
- **Response**: `200 OK`

## Data Models

### Company
```json
{
  "id": "guid",
  "name": "string",
  "documentNumber": "string",
  "documentType": "int (1=EIN, 2=CNPJ)",
  "createdAt": "datetime",
  "updatedAt": "datetime"
}
```

### Store
```json
{
  "id": "guid",
  "name": "string",
  "email": "string",
  "phone": "string",
  "address": "string",
  "city": "string",
  "state": "string",
  "zipCode": "string",
  "companyId": "guid",
  "createdAt": "datetime",
  "updatedAt": "datetime"
}
```

### Product
```json
{
  "id": "guid",
  "name": "string",
  "sku": "string",
  "price": "decimal",
  "stock": "int",
  "storeId": "guid",
  "createdAt": "datetime",
  "updatedAt": "datetime"
}
```

## Error Handling

### HTTP Status Codes
- `200 OK`: Successful operation
- `201 Created`: Resource created successfully
- `204 No Content`: Resource deleted successfully
- `400 Bad Request`: Invalid request data
- `404 Not Found`: Resource not found
- `500 Internal Server Error`: Server error

### Error Response Format
```json
{
  "success": false,
  "message": "Error description",
  "errors": [
    {
      "field": "fieldName",
      "message": "validation message"
    }
  ]
}
```

## Validation Rules

Validations have been simplified for demonstration purposes.

## Database Schema

### Azure Functions (Products)
- **Products**: id, name, sku, price, stock, storeId, createdAt, updatedAt

### Stored Procedures
- `GetProductsAsJson(@StoreId)`: Returns products as JSON string
- `GetProductsList(@StoreId)`: Returns products as formatted text

## Testing

Use the provided Postman collection `Quartile_API_Collection.json` for testing all endpoints. The collection includes:

1. Environment variables for base URLs
2. Sample request bodies
3. Proper request sequence (Create → Read → Update → Delete)
4. Cleanup section for proper resource deletion

## Development Setup

1. Clone the repository
2. Restore NuGet packages: `dotnet restore`
3. Run database migrations: `dotnet ef database update`
4. Start Web API: `dotnet run --project src/Quartile.Api`
5. Start Azure Functions: `dotnet run --project src/Quartile.Functions`

## Notes

- CQRS implementation in Companies API is for demonstration purposes
- Azure Functions use Dapper for data access
- Web API uses Entity Framework Core with repository pattern