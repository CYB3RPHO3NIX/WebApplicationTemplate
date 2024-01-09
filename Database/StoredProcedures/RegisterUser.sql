CREATE PROCEDURE [dbo].[InsertUser]
    @Username VARCHAR(500),
    @Password VARCHAR(1000),
    @IsSuccess BIT OUTPUT,
    @Message VARCHAR(500) OUTPUT
AS
BEGIN
    -- Check if the user already exists
    IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Username = @Username)
    BEGIN
        DECLARE @Salt VARCHAR(500);
        DECLARE @PasswordHash VARCHAR(1000);
        DECLARE @DefaultRoleId bigint;

        -- Generate a random salt
        SET @Salt = CAST(NEWID() AS VARCHAR(500));

        -- Hash the password with the generated salt
        SET @PasswordHash = HASHBYTES('SHA2_256', CAST(@Password AS NVARCHAR(MAX)) + @Salt);

        SET @DefaultRoleId = (SELECT RoleId FROM Roles Where IsDefault = 1);
        -- Insert the new user with the generated salt and password hash
        INSERT INTO dbo.Users (Username, PasswordHash, PasswordSalt, RoleId)
        VALUES (@Username, @PasswordHash, @Salt, @DefaultRoleId);

        SET @IsSuccess = 1;
        SET @Message = 'User Inserted Successfully.';
    END
    ELSE
    BEGIN
        SET @IsSuccess = 0;
        SET @Message = 'User already exists.';
    END
END