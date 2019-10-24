-- =============================================
-- Author:		Michael Kühberger
-- Create date: 24/09/2019
-- Description:
-- =============================================
CREATE PROCEDURE [dbo].[OutputNullable]
  @nullable INT OUTPUT
AS
BEGIN
	SET @nullable = null
END