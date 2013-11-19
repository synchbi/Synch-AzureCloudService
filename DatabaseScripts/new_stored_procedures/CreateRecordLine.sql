-- =============================================
-- Author:		Changhao Han
-- Create date: 11/18/2013
-- Description:	
-- =============================================
CREATE PROCEDURE CreateRecordLine
	-- Add the parameters for the stored procedure here
	@recordId int,
	@upc varchar(20),
	@quantity int,
	@price decimal(18,2),
	@note varchar(200)
AS
	insert into RecordLine values(@recordId, @upc, @quantity, @price, @note);
GO