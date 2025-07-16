IF EXISTS (SELECT * FROM sys.objects WHERE type = 'FN' AND name = 'GetProductsList')
    DROP FUNCTION GetProductsList;
GO

CREATE FUNCTION GetProductsList
(
    @StoreId UNIQUEIDENTIFIER = NULL
)
RETURNS NVARCHAR(MAX)
AS
BEGIN
    DECLARE @ProductList NVARCHAR(MAX) = '';
    
    SELECT @ProductList = @ProductList + 
        'ID: ' + CAST(Id AS NVARCHAR(36)) + 
        ', Name: ' + ISNULL(Name, '') + 
        ', SKU: ' + ISNULL(Sku, '') + 
        ', Price: ' + CAST(ISNULL(Price, 0) AS NVARCHAR(20)) + 
        ', Stock: ' + CAST(ISNULL(Stock, 0) AS NVARCHAR(10)) + 
        ' | '
    FROM Products 
    WHERE (@StoreId IS NULL OR StoreId = @StoreId)
    ORDER BY Name;
    
    RETURN ISNULL(@ProductList, 'No products found.');
END
GO 