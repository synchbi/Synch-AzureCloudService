-- =============================================
-- Author:		Changhao Han
-- Create date: 11/18/2013
-- Description:	
-- =============================================
CREATE PROCEDURE DeleteCustomerById
	@customerId int,
	@businessId int
AS
	delete from Customer where businessId = @businessId and customerId = @customerId;
GO