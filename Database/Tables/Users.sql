CREATE TABLE [dbo].[Users]
(
	[UserId] [bigint] PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[Username] [varchar](500) NOT NULL UNIQUE,
	[PasswordHash] [varchar](1000) NOT NULL,
	[PasswordSalt] [varchar](500) NOT NULL, 
    [RoleId] BIGINT NOT NULL,
	CONSTRAINT [FK_Users_Roles] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles] ([RoleId])
)
