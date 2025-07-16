IF EXISTS (SELECT * FROM sys.objects WHERE type = 'FN' AND name = 'GetProductsAsJson')
    DROP FUNCTION GetProductsAsJson;
GO

CREATE FUNCTION GetProductsAsJson
(
    @CompanyId UNIQUEIDENTIFIER = NULL,
    @StoreId UNIQUEIDENTIFIER = NULL
)
RETURNS NVARCHAR(MAX)
AS
BEGIN
    DECLARE @JsonResult NVARCHAR(MAX);
    
    SELECT @JsonResult = (
        SELECT 
            Id,
            Name,
            Sku,
            Price,
            Stock,
            CompanyId,
            StoreId,
            CreatedAt,
            UpdatedAt
        FROM Products 
        WHERE (@CompanyId IS NULL OR CompanyId = @CompanyId)
            AND (@StoreId IS NULL OR StoreId = @StoreId)
        FOR JSON PATH, ROOT('products')
    );
    
    RETURN ISNULL(@JsonResult, '{"products": []}');
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'FN' AND name = 'GetProductsList')
    DROP FUNCTION GetProductsList;
GO

CREATE FUNCTION GetProductsList
(
    @CompanyId UNIQUEIDENTIFIER = NULL,
    @StoreId UNIQUEIDENTIFIER = NULL
)
RETURNS NVARCHAR(MAX)
AS
BEGIN
    DECLARE @ProductList NVARCHAR(MAX) = '';
    
    SELECT @ProductList = @ProductList + 
        'ID: ' + CAST(Id AS NVARCHAR(36)) + 
        ', Name: ' + Name + 
        ', SKU: ' + Sku + 
        ', Price: ' + CAST(Price AS NVARCHAR(20)) + 
        ', Stock: ' + CAST(Stock AS NVARCHAR(10)) + 
        CHAR(13) + CHAR(10)
    FROM Products 
    WHERE (@CompanyId IS NULL OR CompanyId = @CompanyId)
        AND (@StoreId IS NULL OR StoreId = @StoreId)
    ORDER BY Name;
    
    RETURN ISNULL(@ProductList, 'No products found.');
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'InsertProduct')
    DROP PROCEDURE InsertProduct;
GO

CREATE PROCEDURE InsertProduct
    @Id UNIQUEIDENTIFIER,
    @Name NVARCHAR(100),
    @Sku NVARCHAR(50),
    @Price DECIMAL(18,2),
    @Stock INT,
    @CompanyId UNIQUEIDENTIFIER,
    @StoreId UNIQUEIDENTIFIER,
    @CreatedAt DATETIME2,
    @UpdatedAt DATETIME2
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        INSERT INTO Products (Id, Name, Sku, Price, Stock, CompanyId, StoreId, CreatedAt, UpdatedAt)
        VALUES (@Id, @Name, @Sku, @Price, @Stock, @CompanyId, @StoreId, @CreatedAt, @UpdatedAt);
        
        SELECT @Id AS Id;
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
GO 