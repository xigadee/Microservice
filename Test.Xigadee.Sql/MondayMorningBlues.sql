CREATE TABLE [dbo].[MondayMorningBlues]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [ExternalId] UNIQUEIDENTIFIER NOT NULL, 
    [VersionId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(), 
    [NotEnoughCoffee] BIT NOT NULL DEFAULT 0, 
    [NotEnoughSleep] BIT NOT NULL DEFAULT 0, 
    [Message] NVARCHAR(250) NULL, 
    [Email] NVARCHAR(150) NULL, 
    [ContentId] UNIQUEIDENTIFIER NOT NULL,
	[DateCreated] DATETIME NOT NULL DEFAULT(GETUTCDATE()),
	[DateUpdated] DATETIME NULL 
)

GO

CREATE UNIQUE INDEX [IX_MondayMorningBlues_ExternalId] ON [dbo].[MondayMorningBlues] ([ExternalId]) INCLUDE (Id,VersionId)
GO

CREATE UNIQUE INDEX [IX_MondayMorningBlues_Email] ON [dbo].[MondayMorningBlues] ([Email]) INCLUDE (Id,VersionId) WHERE [Email] IS NOT NULL 
GO