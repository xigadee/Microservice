CREATE PROCEDURE [{NamespaceExternal}].[{spSearch}_Default_Json]
	@Body NVARCHAR(MAX)
AS
BEGIN
	BEGIN TRY

	DECLARE @ETag UNIQUEIDENTIFIER, @CollectionId BIGINT, @Result INT, @RecordResult BIGINT

	EXEC @Result = [{NamespaceTable}].[{spSearch}InternalBuild_Json] 
		@Body, @ETag OUTPUT, @CollectionId OUTPUT, @RecordResult OUTPUT

	IF (@RecordResult = 0)
	BEGIN
		RETURN 202;
	END

	DECLARE @Skip INT = ISNULL(CAST(JSON_VALUE(@Body,'lax $.SkipValue') AS INT), 0);
	DECLARE @Top INT = ISNULL(CAST(JSON_VALUE(@Body,'lax $.TopValue') AS INT), 50);

	--Output
	SELECT E.ExternalId, E.VersionId, 
		(
			SELECT PK.[Type] AS 'Type',P.[Value] AS 'Value'
			FROM [{NamespaceTable}].[{EntityName}Property] AS P
			INNER JOIN [{NamespaceTable}].[{EntityName}PropertyKey] PK ON P.KeyId = PK.Id
			WHERE F.Id = P.EntityId
			FOR JSON PATH, ROOT('Property')
		) AS Body
	FROM [{NamespaceTable}].[udf{EntityName}PaginateProperty](@CollectionId, @Body) AS F
	INNER JOIN [{NamespaceTable}].[{EntityName}] AS E ON F.Id = E.Id
	ORDER BY F.[Rank]
	OFFSET @Skip ROWS
	FETCH NEXT @Top ROWS ONLY
	
	RETURN 200;

	END TRY
	BEGIN CATCH
		SELECT ERROR_NUMBER() AS ErrorNumber, ERROR_MESSAGE() AS ErrorMessage; 
		RETURN 500;
	END CATCH
END