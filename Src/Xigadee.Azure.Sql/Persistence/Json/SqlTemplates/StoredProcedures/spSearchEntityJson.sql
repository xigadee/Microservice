CREATE PROCEDURE [{NamespaceExternal}].[{spSearch}Entity_Default_Json]
	@Body NVARCHAR(MAX)
AS
BEGIN
	BEGIN TRY
		--Build
		DECLARE @FilterIds TABLE
		(
			Id BIGINT,
			Score INT
		);

		DECLARE @ETag UNIQUEIDENTIFIER;
		SET @ETag = ISNULL(TRY_CONVERT(UNIQUEIDENTIFIER, JSON_VALUE(@Body,'lax $.ETag')), NEWID());

		DECLARE @Result INT = 405;

		EXEC [{NamespaceTable}].spSearchLog @ETag, '{EntityName}', '{spSearch}Entity_Json', @Body;

		INSERT INTO @FilterIds
			EXEC @Result = [{NamespaceTable}].[{spSearch}InternalBuild_Json] @ETag, @Body

		--Output
		SELECT E.ExternalId, E.VersionId, E.Body 
		FROM @FilterIds AS F
		INNER JOIN [{NamespaceTable}].[{EntityName}] AS E ON F.Id = E.Id
		ORDER BY F.Score;

		RETURN @Result;
	END TRY
	BEGIN CATCH
		SELECT ERROR_NUMBER() AS ErrorNumber, ERROR_MESSAGE() AS ErrorMessage; 
		RETURN 500;
	END CATCH
END