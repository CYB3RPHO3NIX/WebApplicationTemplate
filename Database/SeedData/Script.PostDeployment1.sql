INSERT INTO [dbo].[Roles] ([RoleName] ,[Description], [IsDefault]) VALUES
('USER','Users typically represent regular individuals who use the application. They have access to basic functionality and features but might not have administrative privileges.', 1);

INSERT INTO [dbo].[Roles] ([RoleName] ,[Description], [IsDefault]) VALUES
('ADMINISTRATOR','Administrators have elevated privileges and control over the application. They are responsible for managing and overseeing the system, including user accounts and application settings.',0);