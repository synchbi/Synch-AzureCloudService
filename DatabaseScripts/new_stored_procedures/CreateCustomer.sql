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
	@category int
AS
if not exists (select * from Customer where businessId = @businessId and customerId = @customerId)
begin
	insert into Customer values(@businessId, @customerId, @address, @email, @phoneNumber, @category);
end
else
update Customer set
		address = @address,
		email = @email,
		phoneNumber = @phoneNumber,
		category = @category
	where businessId = @businessId and customerId = @customerId;
GO