CREATE PROCEDURE [dbo].[InsertUser]
    @Username VARCHAR(500),
    @Password VARCHAR(1000),
    @RoleId BIGINT
AS
BEGIN
    -- Check if the user already exists
    IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Username = @Username)
    BEGIN
        DECLARE @Salt VARCHAR(500);
        DECLARE @PasswordHash VARCHAR(1000);

        -- Generate a random salt
        SET @Salt = CAST(NEWID() AS VARCHAR(500));

        -- Hash the password with the generated salt
        SET @PasswordHash = HASHBYTES('SHA2_256', CAST(@Password AS NVARCHAR(MAX)) + @Salt);

        -- Insert the new user with the generated salt and password hash
        INSERT INTO dbo.Users (Username, PasswordHash, PasswordSalt, RoleId)
        VALUES (@Username, @PasswordHash, @Salt, @RoleId);

        IF @@ERROR = 0
            RETURN 1; -- Successful transaction
        ELSE
            RETURN 0; -- Transaction failed
    END
    ELSE
    BEGIN
        -- User already exists, return 0 to indicate failure
        RETURN 0; -- Transaction failed
    END
END