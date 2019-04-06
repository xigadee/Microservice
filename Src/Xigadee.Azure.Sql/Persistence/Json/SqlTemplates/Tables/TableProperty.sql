CREATE TABLE [{NamespaceTable}].[{EntityName}Property]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), 
	[EntityId] BIGINT NOT NULL,
    [KeyId] INT NOT NULL, 
    [Value] NVARCHAR(250) NOT NULL, 
    CONSTRAINT [FK_{EntityName}Property_Id] FOREIGN KEY ([EntityId]) REFERENCES [{NamespaceTable}].[{EntityName}]([Id]), 
    CONSTRAINT [FK_{EntityName}Property_KeyId] FOREIGN KEY ([{EntityName}KeyId]) REFERENCES [{NamespaceTable}].[{EntityName}PropertyKey]([Id])
)
GO
CREATE INDEX [IX_{EntityName}Property_EntityId] ON [{NamespaceTable}].[{EntityName}Property] ([EntityId]) INCLUDE ([Id])
GO
CREATE INDEX [IX_{EntityName}Property_{EntityName}KeyId] ON [{NamespaceTable}].[{EntityName}Property] ([KeyId],[EntityId]) INCLUDE ([Value])
