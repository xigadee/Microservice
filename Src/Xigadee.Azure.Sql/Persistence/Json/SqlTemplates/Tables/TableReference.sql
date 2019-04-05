CREATE TABLE [{NamespaceTable}].[{EntityName}Reference]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), 
	[EntityId] BIGINT NOT NULL,
    [ReferenceKeyId] INT NOT NULL, 
    [Value] NVARCHAR(250) NOT NULL, 
    CONSTRAINT [FK_{EntityName}Reference_{EntityName}Id] FOREIGN KEY ([Id]) REFERENCES [{NamespaceTable}].[{EntityName}]([Id]), 
    CONSTRAINT [FK_{EntityName}Reference_{EntityName}ReferenceKeyId] FOREIGN KEY ([{EntityName}ReferenceKeyId]) REFERENCES [{NamespaceTable}].[{EntityName}ReferenceKey]([Id])
)
GO
