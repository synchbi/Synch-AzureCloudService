-- =============================================
-- Author:		Changhao Han
-- Create date: 11/18/2013
-- Description:	
-- =============================================
CREATE PROCEDURE UpdateSupplier
	-- Add the parameters for the stored procedure here
	@businessId int,
	@supplierId int,
	@address varchar(200),
	@email varchar(200),
	@phoneNumber varchar(20),
	@category int,
	@accountId int,
	@integrationId varchar(32),
	@status int
AS
	update Supplier set
		address = @address,
		email = @email,
		phoneNumber = @phoneNumber,
		category = @category,
		accountId = @accountId,
		integrationId = @integrationId,
		status = @status
	where businessId = @businessId and supplierId = @supplierId;
GO