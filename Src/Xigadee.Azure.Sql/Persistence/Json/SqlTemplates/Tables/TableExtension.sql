--#region.extension
CREATE TABLE [{NamespaceTable}].[{EntityName}_Extension]
(
     [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
     [EntityId] BIGINT NOT NULL,
	 CONSTRAINT [FK_{EntityName}_Extension_EntityId] FOREIGN KEY ([EntityId]) REFERENCES [{NamespaceTable}].[{EntityName}]([Id]), 
)
GO
CREATE UNIQUE INDEX[IX_{EntityName}_Extension_EntityId] ON [{NamespaceTable}].[{EntityName}_Extension] ([EntityId])
GO
--#endregion