-- =============================================
-- Author:		Grégoire Verdier
-- Create date: 29/03/2017
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[ListAll]
	@limit bigint = 9223372036854775807
WITH RECOMPILE  
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	--CHECKPOINT
	--DBCC DROPCLEANBUFFERS

    -- Insert statements for procedure here
	SELECT TOP(@limit) * FROM Table_1;
END