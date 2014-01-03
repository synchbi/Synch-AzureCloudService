-- =============================================
-- Author:		Changhao Han
-- Create date: 11/18/2013
-- Description:	
-- =============================================
CREATE PROCEDURE UpdateProductUpc
	-- Add the parameters for the stored procedure here
	@upc varchar(20),
	@newUpc varchar (20)
AS
	update Product set upc = @newUpc where upc = @upc;
	update RecordLine set upc = @newUpc where upc = @upc;
GO