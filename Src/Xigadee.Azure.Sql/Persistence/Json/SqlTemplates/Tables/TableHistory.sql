CREATE TABLE[{NamespaceTable}].[{EntityName}History]
(
     [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1)
	,[ExternalId] UNIQUEIDENTIFIER NOT NULL
	,[DllVersion] VARCHAR(32) NULL
	,[Body] NVARCHAR(MAX)
	,[DateCreated] DATETIME NOT NULL DEFAULT(GETUTCDATE())
	,[DateUpdated] DATETIME NULL
    ,[VersionId] UNIQUEIDENTIFIER NOT NULL DEFAULT(NEWID())
)

GO
CREATE UNIQUE INDEX[IX_{EntityName}_ExternalId] ON [{NamespaceTable}].[{EntityName}] ([ExternalId])
