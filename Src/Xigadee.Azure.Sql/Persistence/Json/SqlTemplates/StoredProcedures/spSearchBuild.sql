CREATE PROCEDURE [{NamespaceTable}].[{spSearch}InternalBuild_Default]
	@PropertiesFilter [{NamespaceExternal}].[KvpTableType] READONLY,
	@PropertyOrder [{NamespaceExternal}].[KvpTableType] READONLY,
	@Skip INT = 0,
	@Top INT = 50
AS
BEGIN
	DECLARE @ParamCount INT = (SELECT COUNT(*) FROM @PropertiesFilter);

	IF (@Skip IS NULL)
		SET @Skip = 0;
	IF (@Top IS NULL)
		SET @Top = 50;

	IF (@ParamCount = 0)
		SELECT P.Id
		FROM [{NamespaceTable}].[{EntityName}] AS P
		ORDER BY P.Id
		OFFSET @Skip ROWS
		FETCH NEXT @Top ROWS ONLY
	ELSE IF (@ParamCount = 1)
		SELECT P.EntityId As Id
		FROM [{NamespaceTable}].[{EntityName}Property] AS P
		INNER JOIN [{NamespaceTable}].[{EntityName}PropertyKey] PK ON P.KeyId = PK.Id
		INNER JOIN @PropertiesFilter PF ON PF.RefType = PK.[Type] AND PF.RefValue = P.Value
		ORDER BY P.EntityId
		OFFSET @Skip ROWS
		FETCH NEXT @Top ROWS ONLY
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
		ORDER BY R.Id
		OFFSET @Skip ROWS
		FETCH NEXT @Top ROWS ONLY


END