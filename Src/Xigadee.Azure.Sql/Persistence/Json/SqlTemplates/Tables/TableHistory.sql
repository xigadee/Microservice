--#region.tables
CREATE TABLE[{NamespaceTable}].[{EntityName}History]
(
     [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1)
    ,[EntityId] BIGINT NOT NULL 
	,[ExternalId] UNIQUEIDENTIFIER NOT NULL
    ,[VersionId] UNIQUEIDENTIFIER NULL 
    ,[UserIdAudit] UNIQUEIDENTIFIER NULL 
	,[DateCreated] DATETIME NOT NULL 
	,[DateUpdated] DATETIME NULL
	,[TimeStamp] DATETIME NOT NULL DEFAULT(GETUTCDATE())
	,[Sig] VARCHAR(256) NULL
	,[Body] NVARCHAR(MAX) NULL
)
GO
CREATE INDEX [IX_{EntityName}History_EntityId] ON [{NamespaceTable}].[{EntityName}History] ([EntityId])
GO
--#endregion