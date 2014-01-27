-- =============================================
-- Author:		Changhao Han
-- Create date: 11/18/2013
-- Description:	
-- =============================================
CREATE PROCEDURE DeleteRecordById
	@recordId int
AS
	delete from Record where id = @recordId;
GO