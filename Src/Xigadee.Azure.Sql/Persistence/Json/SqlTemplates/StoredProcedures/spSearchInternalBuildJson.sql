CREATE PROCEDURE [{NamespaceTable}].[{spSearch}InternalBuild_Json]
	@ETag UNIQUEIDENTIFIER,
	@Body NVARCHAR(MAX)
AS
BEGIN
	DECLARE @Skip INT = ISNULL(CAST(JSON_VALUE(@Body,'lax $.SkipValue') AS INT), 0);
	DECLARE @Top INT = ISNULL(CAST(JSON_VALUE(@Body,'lax $.TopValue') AS INT), 50);

	SELECT P.Id AS [Id], 1 AS [Score]
	FROM [{NamespaceTable}].[{EntityName}] AS P
	ORDER BY P.Id
	OFFSET @Skip ROWS
	FETCH NEXT @Top ROWS ONLY;

	--IF (@ParamCount = 0)
	--	SELECT P.Id
	--	FROM [{NamespaceTable}].[{EntityName}] AS P
	--	ORDER BY P.Id
	--	OFFSET @Skip ROWS
	--	FETCH NEXT @Top ROWS ONLY
	--ELSE IF (@ParamCount = 1)
	--	SELECT P.EntityId As Id
	--	FROM [{NamespaceTable}].[{EntityName}Property] AS P
	--	INNER JOIN [{NamespaceTable}].[{EntityName}PropertyKey] PK ON P.KeyId = PK.Id
	--	INNER JOIN @PropertiesFilter PF ON PF.RefType = PK.[Type] AND PF.RefValue = P.Value
	--	ORDER BY P.EntityId
	--	OFFSET @Skip ROWS
	--	FETCH NEXT @Top ROWS ONLY
	--ELSE
	--	SELECT R.Id
	--	FROM
	--	(
	--		SELECT P.EntityId As Id, 1 AS Num
	--		FROM [{NamespaceTable}].[{EntityName}Property] AS P
	--		INNER JOIN [{NamespaceTable}].[{EntityName}PropertyKey] PK ON P.KeyId = PK.Id
	--		INNER JOIN @PropertiesFilter PF ON PF.RefType = PK.[Type] AND PF.RefValue = P.Value
	--	)AS R
	--	GROUP BY R.Id
	--	HAVING SUM(R.Num) = @ParamCount
	--	ORDER BY R.Id
	--	OFFSET @Skip ROWS
	--	FETCH NEXT @Top ROWS ONLY

	RETURN 200;
END