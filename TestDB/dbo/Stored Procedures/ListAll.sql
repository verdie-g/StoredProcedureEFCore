-- =============================================
-- Author:		Grégoire Verdier
-- Create date: 29/03/2017
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[ListAll]
	@p bigint = 0
WITH RECOMPILE  
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	--CHECKPOINT
	--DBCC DROPCLEANBUFFERS

    -- Insert statements for procedure here
	SELECT * FROM Table_1;
END