CREATE TABLE [{NamespaceTable}].[{EntityName}SearchHistory]
(
     [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1)
	,[ETag] UNIQUEIDENTIFIER NOT NULL
	,[EntityType] VARCHAR(50) NOT NULL
	,[SearchType] VARCHAR(50) NOT NULL
	,[TimeStamp] DATETIME NOT NULL DEFAULT(GETUTCDATE())
	,[Sig] VARCHAR(256) NULL
	,[Body] NVARCHAR(MAX) NULL
	,[HistoryIndex] BIGINT NULL
)
GO 
CREATE UNIQUE INDEX[IX_{EntityName}SearchHistory_ETag] ON [{NamespaceTable}].[{EntityName}SearchHistory] ([ETag]) INCLUDE([HistoryIndex])
GO
CREATE TABLE [{NamespaceTable}].[{EntityName}SearchHistoryCache]
(
     [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1)
    ,[SearchId] BIGINT NOT NULL 
    ,[EntityId] BIGINT NOT NULL 
	,[Score] BIGINT NOT NULL DEFAULT(-1)
	,CONSTRAINT [FK_{EntityName}SearchHistoryCache_SearchId] FOREIGN KEY ([SearchId]) REFERENCES [{NamespaceTable}].[{EntityName}SearchHistory]([Id]), 
)
GO
CREATE INDEX[IX_{EntityName}SearchHistoryCache_SearchHistory] ON [{NamespaceTable}].[{EntityName}SearchHistoryCache] ([SearchId],[EntityId]) INCLUDE ([Score]) 
GO
