CREATE PROCEDURE [dbo].[ListNotAll]
	@limit bigint = 9223372036854775807
AS
BEGIN
	SET NOCOUNT ON;

	SELECT TOP(@limit) id, name, 5 AS extra_column FROM Table_1;
END