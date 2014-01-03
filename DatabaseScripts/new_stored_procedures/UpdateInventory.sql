-- =============================================
-- Author:		Changhao Han
-- Create date: 11/18/2013
-- Description:	
-- =============================================
CREATE PROCEDURE UpdateInventory
	-- Add the parameters for the stored procedure here
	@businessId int,
	@upc varchar(20),
	@name varchar(100),
	@defaultPrice decimal(18,2),
	@detail varchar(200),
	@leadTime int,
	@quantityAvailable int,
	@reorderQuantity int,
	@reorderPoint int,
	@category int,
	@location varchar(40),
	@quantityOnPurchaseOrder int
AS
	update Inventory set
		name = @name,
		defaultPrice = @defaultPrice,
		detail = @detail,
		leadTime = @leadTime,
		quantityAvailable = @quantityAvailable,
		reorderQuantity = @reorderQuantity,
		reorderPoint = @reorderPoint,
		category = @category,
		location = @location,
		quantityOnPurchaseOrder = @quantityOnPurchaseOrder
	where businessId = @businessId and upc = @upc;
GO