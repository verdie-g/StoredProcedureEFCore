-- =============================================
-- Author:		Grégoire Verdier
-- Create date: 29/03/2017
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[ListAll]
	@limit bigint = 9223372036854775807,
  @limitOut bigint = 0 OUTPUT
AS
BEGIN
	SET NOCOUNT ON;

  SET @limitOut = @limit;

	SELECT TOP(@limit) *, 5 AS extra_column FROM Table_1;
END