-- =============================================
-- Author:  Changhao Han
-- Create date: 11/18/2013
-- Description: 
-- =============================================
CREATE PROCEDURE GetCustomerById
 @businessId int,
 @customerId int
AS
 select Business.name, Business.postalCode, Customer.* from Business, Customer
 where Customer.businessId = @businessId and Business.id = Customer.customerId and Customer.customerId = @customerId;
GO