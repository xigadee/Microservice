CREATE TABLE [{NamespaceTable}].[SearchHistory]
(
     [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1)
	,[ETag] UNIQUEIDENTIFIER NOT NULL
	,[EntityType] VARCHAR(50) NOT NULL
	,[SearchType] VARCHAR(50) NOT NULL
	,[TimeStamp] DATETIME NOT NULL DEFAULT(GETUTCDATE())
	,[Sig] VARCHAR(256) NULL
	,[Body] NVARCHAR(MAX) NULL
)
GO 
CREATE UNIQUE INDEX[IX_SearchHistory_ETag] ON [{NamespaceTable}].[SearchHistory] ([ETag]) 
GO
CREATE TABLE [{NamespaceTable}].[SearchHistoryCache]
(
     [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1)
    ,[SearchId] BIGINT NOT NULL 
    ,[EntityId] BIGINT NOT NULL 
	,[Score] INT NOT NULL
	,CONSTRAINT [FK_SearchHistoryCache_SearchId] FOREIGN KEY ([SearchId]) REFERENCES [{NamespaceTable}].[SearchHistory]([Id]), 
)
GO
CREATE INDEX[IX_SearchHistoryCache_SearchHistory] ON [{NamespaceTable}].[SearchHistoryCache] ([SearchId],[EntityId]) INCLUDE ([Score]) 
GO
