--#region.tables
CREATE TABLE [{NamespaceTable}].[{EntityName}Reference]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), 
	[EntityId] BIGINT NOT NULL,
    [KeyId] INT NOT NULL, 
    [Value] NVARCHAR(250) NOT NULL, 
    CONSTRAINT [FK_{EntityName}Reference_Id] FOREIGN KEY ([EntityId]) REFERENCES [{NamespaceTable}].[{EntityName}]([Id]), 
    CONSTRAINT [FK_{EntityName}Reference_KeyId] FOREIGN KEY ([KeyId]) REFERENCES [{NamespaceTable}].[{EntityName}ReferenceKey]([Id])
)
GO
CREATE UNIQUE INDEX [IX_{EntityName}Reference_TypeReference] ON [{NamespaceTable}].[{EntityName}Reference] ([KeyId],[Value]) INCLUDE ([EntityId])
GO
CREATE INDEX [IX_{EntityName}Reference_EntityId] ON [{NamespaceTable}].[{EntityName}Reference] ([EntityId]) INCLUDE ([Id])
GO
CREATE INDEX [IX_{EntityName}Reference_KeyId] ON [{NamespaceTable}].[{EntityName}Reference] ([KeyId],[EntityId]) INCLUDE ([Value])
GO
--#endregion