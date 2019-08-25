--#region.tables
CREATE TABLE [{NamespaceTable}].[{EntityName}ReferenceKey]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [Type] VARCHAR(20) NULL
)
GO

CREATE UNIQUE INDEX [IX_{EntityName}ReferenceKey_Type] ON [{NamespaceTable}].[{EntityName}ReferenceKey] ([Type])
GO
--#endregion