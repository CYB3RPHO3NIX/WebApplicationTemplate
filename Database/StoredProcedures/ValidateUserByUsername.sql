CREATE PROCEDURE [dbo].[ValidateUserByUsername]
    @Username VARCHAR(500),
    @Password VARCHAR(1000),
    @UserId BIGINT OUTPUT,
    @IsValid BIT OUTPUT,
    @Message VARCHAR(500) OUTPUT
AS
BEGIN
    DECLARE @StoredPasswordHash VARCHAR(1000);
    DECLARE @StoredPasswordSalt VARCHAR(500);
    SET @UserId = 0;
    -- Get PasswordHash and PasswordSalt based on the provided Username
    SELECT
        @StoredPasswordHash = PasswordHash,
        @StoredPasswordSalt = PasswordSalt
    FROM dbo.Users
    WHERE Username = @Username;

    -- Check if the user exists
    IF @StoredPasswordHash IS NOT NULL
    BEGIN
        -- Verify the password by hashing the provided password with the stored salt
        DECLARE @InputPasswordHash VARCHAR(1000);
        SET @InputPasswordHash = HASHBYTES('SHA2_256', CAST(@Password AS NVARCHAR(MAX)) + @StoredPasswordSalt);

        -- Check if the input password hash matches the stored password hash
        IF @InputPasswordHash = @StoredPasswordHash
        BEGIN
            SELECT @UserId = UserId FROM dbo.Users WHERE Username = @Username;
            SET @Message = 'Valid User';
            SET @IsValid = 1;
        END
        ELSE
        BEGIN
            SET @Message = 'Invalid Username/password';
            SET @IsValid = 0;
        END
    END
    ELSE
    BEGIN
        SET @Message = 'No such user exists.';
        SET @IsValid = 0;
    END
END