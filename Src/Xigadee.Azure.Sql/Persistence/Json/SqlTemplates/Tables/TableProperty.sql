CREATE TABLE [{NamespaceTable}].[{EntityName}Property]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), 
	[EntityId] BIGINT NOT NULL,
    [KeyId] INT NOT NULL, 
    [Value] NVARCHAR(250) NOT NULL, 
    CONSTRAINT [FK_{EntityName}Property_Id] FOREIGN KEY ([EntityId]) REFERENCES [{NamespaceTable}].[{EntityName}]([Id]), 
    CONSTRAINT [FK_{EntityName}Property_KeyId] FOREIGN KEY ([KeyId]) REFERENCES [{NamespaceTable}].[{EntityName}PropertyKey]([Id])
)
GO
CREATE INDEX [IX_{EntityName}Property_KeyId] ON [{NamespaceTable}].[{EntityName}Property] ([KeyId]) INCLUDE ([EntityId])
GO
CREATE INDEX [IX_{EntityName}Property_EntityId] ON [{NamespaceTable}].[{EntityName}Property] ([EntityId]) INCLUDE ([Id])
