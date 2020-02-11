CREATE PROCEDURE [dbo].[output_nullable]
  @nullable_in INT,
  @nullable_out INT OUTPUT
AS
BEGIN
    SET @nullable_out = @nullable_in;
END