--#region.extension
CREATE VIEW [{NamespaceTable}].[View{EntityName}]
AS
	SELECT E.[Id] 
	,E.[ExternalId] 
    ,E.[VersionId] 
    ,E.[UserIdAudit] 
	,E.[DateCreated] 
	,E.[DateUpdated] 
	,E.[Sig] 
	,E.[Body] 
	FROM [{NamespaceTable}].[{EntityName}] AS E 
	INNER JOIN [{NamespaceTable}].[{EntityName}_Extension] AS EX ON E.Id = EX.EntityId
GO
--#endregion