-- =============================================
-- Author:		Changhao Han
-- Create date: 11/18/2013
-- Description:	
-- =============================================
CREATE PROCEDURE DeleteSupplierById
	@supplierId int,
	@businessId int
AS
	delete from Supplier where businessId = @businessId and supplierId = @supplierId;
GO