-- =============================================
-- Author:  Changhao Han
-- Create date: 11/18/2013
-- Description: 
-- =============================================
CREATE PROCEDURE GetCompleteRecords
 @ownerId int
AS
 select * from Record, RecordLine
 where Record.ownerId = @ownerId and RecordLine.recordId = Record.id
 order by transactionDate desc;
GO