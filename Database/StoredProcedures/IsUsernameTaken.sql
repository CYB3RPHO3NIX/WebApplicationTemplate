CREATE PROCEDURE [dbo].[IsUsernameTaken]
	@Username VARCHAR(500),
	@IsTaken BIT OUTPUT
AS
BEGIN
	SET NOCOUNT ON;

	IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Username = @Username)
        SET @IsTaken = 0; -- Username is unique
	ELSE
		SET @IsTaken = 1; -- Username is Taken
END