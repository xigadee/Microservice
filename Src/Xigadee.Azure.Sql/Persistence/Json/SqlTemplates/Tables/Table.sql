CREATE TABLE[{NamespaceTable}].[{EntityName}]
(
     [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1)
	,[ExternalId] UNIQUEIDENTIFIER NOT NULL
    ,[VersionId] UNIQUEIDENTIFIER NULL 
	,[DateCreated] DATETIME NOT NULL DEFAULT(GETUTCDATE())
	,[DateUpdated] DATETIME NULL
	,[IsDeleted] BIT NOT NULL DEFAULT(0)
	,[Body] NVARCHAR(MAX)
)
GO
CREATE UNIQUE INDEX[IX_{EntityName}_ExternalId] ON [{NamespaceTable}].[{EntityName}] ([ExternalId])
