--#region.extension
CREATE VIEW [{NamespaceTable}].[View{EntityName}]
AS
	SELECT *
	FROM [{NamespaceTable}].[{EntityName}] AS E 
	INNER JOIN [{NamespaceTable}].[{EntityName}_Extended] AS EX ON E.Id = EX.EntityId
GO
--#endregion