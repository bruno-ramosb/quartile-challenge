IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'InsertProduct')
    DROP PROCEDURE InsertProduct;
GO

CREATE PROCEDURE InsertProduct
    @Id UNIQUEIDENTIFIER,
    @Name NVARCHAR(100),
    @Sku NVARCHAR(50),
    @Price DECIMAL(18,2),
    @Stock INT,
    @StoreId UNIQUEIDENTIFIER,
    @CreatedAt DATETIME2,
    @UpdatedAt DATETIME2
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        INSERT INTO Products (Id, Name, Sku, Price, Stock, StoreId, CreatedAt, UpdatedAt)
        VALUES (@Id, @Name, @Sku, @Price, @Stock, @StoreId, @CreatedAt, @UpdatedAt);
        
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