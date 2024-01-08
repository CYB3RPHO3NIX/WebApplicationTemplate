CREATE PROCEDURE [dbo].[ValidateUser]
    @Username VARCHAR(500),
    @Password VARCHAR(1000)
AS
BEGIN
    DECLARE @StoredPasswordHash VARCHAR(1000);
    DECLARE @StoredPasswordSalt VARCHAR(500);

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
            SELECT @Username AS 'ValidUser'; -- Return the username for a valid user
        ELSE
            PRINT 'Invalid Username/password';
    END
    ELSE
        PRINT 'Invalid Username/password';
END