-- =============================================
-- Author:		Grégoire Verdier
-- Create date: 21/04/2018
-- Description:
-- =============================================
CREATE PROCEDURE [dbo].[OutputFixedSize]
  @fixed_size varchar(255) OUTPUT
AS
BEGIN
	SET @fixed_size = 'Jambon Beurre'
END