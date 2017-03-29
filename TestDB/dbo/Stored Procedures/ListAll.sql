-- =============================================
-- Author:		Grégoire Verdier
-- Create date: 29/03/2017
-- Description:	
-- =============================================
CREATE PROCEDURE ListAll
	@p bigint = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * FROM Table_1;
END