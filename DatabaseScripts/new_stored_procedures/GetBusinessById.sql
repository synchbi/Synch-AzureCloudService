-- =============================================
-- Author:		Changhao Han
-- Create date: 11/18/2013
-- Description:	
-- =============================================
CREATE PROCEDURE GetBusinessById
	@businessId int
AS
	select * from Business where id = @businessId;
GO