-- =============================================
-- Author:  Changhao Han
-- Create date: 11/18/2013
-- Description: 
-- =============================================
CREATE PROCEDURE GetSupplierById
 @businessId int,
 @supplierId int
AS
 select Business.name, Business.postalCode, Supplier.* from Business, Supplier
 where Supplier.businessId = @businessId and Business.id = Supplier.supplierId and Supplier.supplierId = @supplierId;
GO