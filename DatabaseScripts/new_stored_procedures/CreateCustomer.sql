-- =============================================
-- Author:		Changhao Han
-- Create date: 11/18/2013
-- Description:	
-- =============================================
CREATE PROCEDURE CreateCustomer
	-- Add the parameters for the stored procedure here
	@businessId int,
	@customerId int,
	@address varchar(200),
	@email varchar(200),
	@phoneNumber varchar(20),
	@category int,
	@accountId int,
	@integrationId varchar(32),
	@status int
AS
if not exists (select * from Customer where businessId = @businessId and customerId = @customerId)
begin
	insert into Customer values(@businessId, @customerId, @address, @email, @phoneNumber, @category, @accountId, @integrationId, @status);
end
else
update Customer set
		address = @address,
		email = @email,
		phoneNumber = @phoneNumber,
		category = @category,
		accountId = @accountId,
		integrationId = @integrationId,
		status = @status
	where businessId = @businessId and customerId = @customerId;
GO