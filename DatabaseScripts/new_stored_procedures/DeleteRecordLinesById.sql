-- =============================================
-- Author:		Changhao Han
-- Create date: 11/18/2013
-- Description:	
-- =============================================
CREATE PROCEDURE DeleteRecordLinesById
	@recordId int
AS
	delete from RecordLine where recordId = @recordId;
GO