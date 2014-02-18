-- =============================================
-- Author:		Changhao Han
-- Create date: 11/18/2013
-- Description:	count number of records
-- =============================================
CREATE PROCEDURE UpdateRecord
	-- Add the parameters for the stored procedure here
	@recordId int,
	@status int,
	@title varchar(50),
	@comment varchar(140),
	@deliveryDate datetimeoffset(7),
	@integrationId varchar(140)
AS
update Record set 
	status = @status,
	title = @title,
	comment = @comment,
	deliveryDate = @deliveryDate,
	integrationId = @integrationId
where id = @recordId;
GO