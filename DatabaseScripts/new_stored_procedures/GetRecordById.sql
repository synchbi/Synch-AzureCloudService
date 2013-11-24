-- =============================================
-- Author:  Changhao Han
-- Create date: 11/18/2013
-- Description: 
-- =============================================
CREATE PROCEDURE GetRecordById
 @ownerId int,
 @recordId int
AS
 select * from Record
 where ownerId = @ownerId and id = @recordId;
GO