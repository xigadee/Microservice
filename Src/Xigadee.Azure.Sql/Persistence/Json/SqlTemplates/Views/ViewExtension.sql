--#region.extension
CREATE VIEW [{NamespaceTable}].[View{EntityName}]
AS
	SELECT E.*
	FROM [{NamespaceTable}].[{EntityName}] AS E 
	INNER JOIN [{NamespaceTable}].[{EntityName}_Extension] AS EX ON E.Id = EX.EntityId
GO
--#endregion