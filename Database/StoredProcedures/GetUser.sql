CREATE PROCEDURE [dbo].[GetUser]
    @UserId BIGINT
AS
BEGIN
    SELECT 
        Username
    FROM dbo.Users
    WHERE UserId = @UserId;
END