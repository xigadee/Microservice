CREATE TABLE [dbo].[Test1ReferenceKey]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [Type] VARCHAR(20) NULL
)
GO

CREATE UNIQUE INDEX [IX_Test1ReferenceKey_Type] ON [dbo].[Test1ReferenceKey] ([Type])
GO
CREATE TABLE [dbo].[Test1PropertyKey]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [Type] VARCHAR(20) NULL
)
GO

CREATE UNIQUE INDEX [IX_Test1PropertyKey_Type] ON [dbo].[Test1PropertyKey] ([Type])
GO
CREATE TABLE[dbo].[Test1]
(
     [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1)
	,[ExternalId] UNIQUEIDENTIFIER NOT NULL
    ,[VersionId] UNIQUEIDENTIFIER NULL 
	,[DateCreated] DATETIME NOT NULL DEFAULT(GETUTCDATE())
	,[DateUpdated] DATETIME NULL
	,[Sig] VARCHAR(256) NULL
	,[Body] NVARCHAR(MAX)
)
GO
CREATE UNIQUE INDEX[IX_Test1_ExternalId] ON [dbo].[Test1] ([ExternalId]) INCLUDE ([VersionId])

GO
CREATE TABLE[dbo].[Test1History]
(
     [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1)
    ,[EntityId] BIGINT NOT NULL 
	,[ExternalId] UNIQUEIDENTIFIER NOT NULL
    ,[VersionId] UNIQUEIDENTIFIER NULL 
	,[DateCreated] DATETIME NOT NULL 
	,[DateUpdated] DATETIME NULL
	,[TimeStamp] DATETIME NOT NULL DEFAULT(GETUTCDATE())
	,[Sig] VARCHAR(256) NULL
	,[Body] NVARCHAR(MAX) NULL
)

GO
CREATE TABLE [dbo].[Test1Property]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), 
	[EntityId] BIGINT NOT NULL,
    [KeyId] INT NOT NULL, 
    [Value] NVARCHAR(250) NOT NULL, 
    CONSTRAINT [FK_Test1Property_Id] FOREIGN KEY ([EntityId]) REFERENCES [dbo].[Test1]([Id]), 
    CONSTRAINT [FK_Test1Property_KeyId] FOREIGN KEY ([KeyId]) REFERENCES [dbo].[Test1PropertyKey]([Id])
)
GO
CREATE INDEX [IX_Test1Property_EntityId] ON [dbo].[Test1Property] ([EntityId]) INCLUDE ([Id])
GO
CREATE INDEX [IX_Test1Property_KeyId] ON [dbo].[Test1Property] ([KeyId],[EntityId]) INCLUDE ([Value])

GO
CREATE TABLE [dbo].[Test1Reference]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), 
	[EntityId] BIGINT NOT NULL,
    [KeyId] INT NOT NULL, 
    [Value] NVARCHAR(250) NOT NULL, 
    CONSTRAINT [FK_Test1Reference_Id] FOREIGN KEY ([EntityId]) REFERENCES [dbo].[Test1]([Id]), 
    CONSTRAINT [FK_Test1Reference_KeyId] FOREIGN KEY ([KeyId]) REFERENCES [dbo].[Test1ReferenceKey]([Id])
)
GO
CREATE UNIQUE INDEX [IX_Test1Reference_TypeReference] ON [dbo].[Test1Reference] ([KeyId],[Value]) INCLUDE ([EntityId])
GO
CREATE INDEX [IX_Test1Reference_EntityId] ON [dbo].[Test1Reference] ([EntityId]) INCLUDE ([Id])
GO
CREATE INDEX [IX_Test1Reference_KeyId] ON [dbo].[Test1Reference] ([KeyId],[EntityId]) INCLUDE ([Value])

GO
