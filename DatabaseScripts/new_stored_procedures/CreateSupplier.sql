-- =============================================
-- Author:		Changhao Han
-- Create date: 11/18/2013
-- Description:	
-- =============================================
CREATE PROCEDURE CreateSupplier
	-- Add the parameters for the stored procedure here
	@businessId int,
	@supplierId int,
	@address varchar(200),
	@email varchar(200),
	@phoneNumber varchar(20),
	@category int,
	@accountId int
AS
if not exists (select * from Supplier where businessId = @businessId and supplierId = @supplierId)
begin
	insert into Supplier values(@businessId, @supplierId, @address, @email, @phoneNumber, @category, @accountId);
end
else
update Supplier set
		address = @address,
		email = @email,
		phoneNumber = @phoneNumber,
		category = @category
	where businessId = @businessId and supplierId = @supplierId;
GO