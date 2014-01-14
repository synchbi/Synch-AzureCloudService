-- =============================================
-- Author:		Changhao Han
-- Create date: 11/18/2013
-- Description:	
-- =============================================
CREATE PROCEDURE UpdateProductUpc
	-- Add the parameters for the stored procedure here
	@upc varchar(20),
	@newUpc varchar (20),
	@businessId int
AS
if not exists (select * from Product where upc = @newUpc)
begin
 update Product set upc = @newUpc where upc = @upc;
 update RecordLine set upc = @newUpc where upc = @upc;
end
else
begin
 if not exists (select * from Inventory where upc = @newUpc and businessId = @businessId)
 begin
  update Inventory set upc = @newUpc where upc = @upc and businessId = @businessId;
  update RecordLine set upc = @newUpc from RecordLine, Record where RecordLine.upc = @upc and RecordLine.recordId = Record.Id and Record.ownerId = @businessId;
 end
end
GO