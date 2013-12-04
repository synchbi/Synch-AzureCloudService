-- =============================================
-- Author:		Changhao Han
-- Create date: 11/18/2013
-- Description:	
-- =============================================
CREATE PROCEDURE GetBusinessByNameAndPostalCode
	-- Add the parameters for the stored procedure here
	@name varchar(100),
	@postalCode varchar(10)
AS
	select * from Business where name = @name and postalCode = @postalCode;
GO