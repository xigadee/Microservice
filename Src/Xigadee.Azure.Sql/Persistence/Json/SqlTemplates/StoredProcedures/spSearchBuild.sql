CREATE PROCEDURE [{NamespaceTable}].[{spSearch}InternalBuild_Default]
	@PropertiesFilter [{NamespaceExternal}].[KvpTableType] READONLY,
	@PropertyOrder [{NamespaceExternal}].[KvpTableType] READONLY,
	@Skip INT = 0,
	@Top INT = 50
AS
BEGIN
	DECLARE @ParamCount INT = (SELECT COUNT(*) FROM @PropertiesFilter);

	IF (@ParamCount = 0)
		SELECT P.Id
		FROM [{NamespaceTable}].[{EntityName}] AS P
	ELSE IF (@ParamCount = 1)
		SELECT P.EntityId As Id
		FROM [{NamespaceTable}].[{EntityName}Property] AS P
		INNER JOIN [{NamespaceTable}].[{EntityName}PropertyKey] PK ON P.KeyId = PK.Id
		INNER JOIN @PropertiesFilter PF ON PF.RefType = PK.[Type] AND PF.RefValue = P.Value
	ELSE
		SELECT R.Id
		FROM
		(
			SELECT P.EntityId As Id, 1 AS Num
			FROM [{NamespaceTable}].[{EntityName}Property] AS P
			INNER JOIN [{NamespaceTable}].[{EntityName}PropertyKey] PK ON P.KeyId = PK.Id
			INNER JOIN @PropertiesFilter PF ON PF.RefType = PK.[Type] AND PF.RefValue = P.Value
		)AS R
		GROUP BY R.Id
		HAVING SUM(R.Num) = @ParamCount


END