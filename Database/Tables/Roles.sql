CREATE TABLE [dbo].[Roles](
	[RoleId] [bigint] PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[RoleName] [varchar](100) NOT NULL UNIQUE,
	[Description] [varchar](1000) NULL, 
    [IsDefault] BIT NOT NULL,
)