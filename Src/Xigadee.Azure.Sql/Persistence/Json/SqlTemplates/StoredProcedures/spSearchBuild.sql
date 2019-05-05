CREATE PROCEDURE [{NamespaceTable}].[{spSearch}InternalBuild_Default]
	@PropertiesFilter [{NamespaceExternal}].[KvpTableType] READONLY,
	@PropertyOrder [{NamespaceExternal}].[KvpTableType] READONLY,
	@Skip INT = 0,
	@Top INT = 50
AS
BEGIN
	DECLARE @ParamCount INT = (SELECT COUNT(*) FROM @PropertiesFilter);

	IF (@ParamCount = 1)
		SELECT P.EntityId As Id
		FROM [dbo].[Test1Property] AS P
		INNER JOIN [dbo].[Test1PropertyKey] PK ON P.KeyId = PK.Id
		INNER JOIN @PropertiesFilter PF ON PF.RefType = PK.[Type] AND PF.RefValue = P.Value
	ELSE
		SELECT R.Id
		FROM
		(
			SELECT P.EntityId As Id, 1 AS Num
			FROM [dbo].[Test1Property] AS P
			INNER JOIN [dbo].[Test1PropertyKey] PK ON P.KeyId = PK.Id
			INNER JOIN @PropertiesFilter PF ON PF.RefType = PK.[Type] AND PF.RefValue = P.Value
		)AS R
		GROUP BY R.Id
		HAVING SUM(R.Num) = @ParamCount


END