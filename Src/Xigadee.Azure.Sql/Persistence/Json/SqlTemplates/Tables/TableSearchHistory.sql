CREATE TABLE [{NamespaceTable}].[SearchHistory]
(
     [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1)
	,[ETag] UNIQUEIDENTIFIER NOT NULL
	,[EntityType] VARCHAR(50)
	,[SearchType] VARCHAR(50)
	,[TimeStamp] DATETIME NOT NULL DEFAULT(GETUTCDATE())
	,[Sig] VARCHAR(256) NULL
	,[Body] NVARCHAR(MAX) NULL
)
GO 
CREATE UNIQUE INDEX[IX_SearchHistory_ETag] ON [{NamespaceTable}].[SearchHistory] ([ETag]) 
