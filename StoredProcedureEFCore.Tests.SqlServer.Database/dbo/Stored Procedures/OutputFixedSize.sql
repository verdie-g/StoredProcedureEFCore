-- =============================================
-- Author:		Grégoire Verdier
-- Create date: 21/04/2018
-- Description:
-- =============================================
CREATE PROCEDURE [dbo].[OutputFixedSize]
  @fixed_size varchar OUTPUT
AS
BEGIN
	SET NOCOUNT ON;

  SET @fixed_size = 'Jambon Beurre'
END