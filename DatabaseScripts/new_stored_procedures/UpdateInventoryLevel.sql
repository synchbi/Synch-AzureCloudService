-- =============================================
-- Author:		Changhao Han
-- Create date: 11/18/2013
-- Description:	
-- =============================================
CREATE PROCEDURE UpdateInventoryLevel
	-- Add the parameters for the stored procedure here
	@businessId int,
	@upc varchar(20),
	@quantityAvailable int
AS
	update Inventory set
		quantityAvailable = @quantityAvailable
	where businessId = @businessId and upc = @upc;
GO