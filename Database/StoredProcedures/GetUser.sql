CREATE PROCEDURE [dbo].[GetUser]
    @UserId BIGINT
AS
BEGIN
    SELECT 
        UserId,
        Username,
        EmailId,
        r.RoleName as 'Role'
    FROM dbo.Users u
    INNER JOIN dbo.Roles r ON u.RoleId = r.RoleId
    WHERE UserId = @UserId;
END