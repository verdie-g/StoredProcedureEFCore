-- =============================================
-- Author:		Grégoire Verdier
-- Create date: 18/04/2017
-- Description:	Returns a boolean
-- =============================================
CREATE PROCEDURE [dbo].[ReturnBoolean] 
	-- Add the parameters for the stored procedure here
	@boolean_to_return bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	RETURN @boolean_to_return;
END