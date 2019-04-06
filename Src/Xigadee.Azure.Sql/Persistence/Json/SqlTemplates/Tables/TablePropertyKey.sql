CREATE TABLE [{NamespaceTable}].[{EntityName}PropertyKey]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [Type] VARCHAR(20) NULL
)
GO

CREATE UNIQUE INDEX [IX_{EntityName}PropertyKey_Type] ON [{NamespaceTable}].[{EntityName}PropertyKey] ([Type])