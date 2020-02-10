CREATE PROCEDURE [{NamespaceTable}].[{spSearch}InternalBuild_Json]
	@Body NVARCHAR(MAX),
	@ETag UNIQUEIDENTIFIER OUTPUT,
	@CollectionId BIGINT OUTPUT,
	@RecordResult BIGINT OUTPUT,
	@CacheHit INT OUTPUT,
	@FullScan BIT OUTPUT
AS
BEGIN
	DECLARE @HistoryIndexId BIGINT, @TimeStamp DATETIME
	SET @ETag = TRY_CONVERT(UNIQUEIDENTIFIER, JSON_VALUE(@Body,'lax $.ETag'));
	DECLARE @ETagDoNotInvalidate BIT = CAST(JSON_VALUE(@Body,'lax $.ETagDoNotInvalidate') AS BIT);

	DECLARE @ParamsCount INT = ISNULL(TRY_CONVERT(INT, JSON_VALUE(@Body,'lax $.ParamsFilter.Count')),0);

	SET @CacheHit = 0;

	--OK, we need to check that the collection is still valid.
	DECLARE @CurrentHistoryIndexId BIGINT = (SELECT TOP 1 Id FROM [{NamespaceTable}].[{EntityName}History] ORDER BY Id DESC);

	IF (@ParamsCount > 0 AND @ETag IS NOT NULL)
	BEGIN
		--OK, check whether the ETag is already assigned to a results set
		SELECT TOP 1 @CollectionId = Id, @HistoryIndexId = [HistoryIndex], @TimeStamp = [TimeStamp], @RecordResult = [RecordCount]
		FROM [{NamespaceTable}].[{EntityName}SearchHistory] 
		WHERE ETag = @ETag;

		IF (@CollectionId IS NOT NULL 
			AND @CurrentHistoryIndexId IS NOT NULL
			AND @HistoryIndexId IS NOT NULL
			AND (@HistoryIndexId = @CurrentHistoryIndexId OR @ETagDoNotInvalidate=1))
		BEGIN
			RETURN 202;
		END
	END

	--We need to create a new search collection and change the ETag.
	SET @ETag = NEWID();

	INSERT INTO [{NamespaceTable}].[{EntityName}SearchHistory]
	([ETag],[EntityType],[SearchType],[Sig],[Body],[HistoryIndex])
	VALUES
	(@ETag, '{EntityName}', '{spSearch}InternalBuild_Json', '', @Body, @CurrentHistoryIndexId);

	SET @CollectionId = @@IDENTITY;

	IF (@ParamsCount = 0)
	BEGIN
		SET @RecordResult = (SELECT COUNT(*) FROM  [{NamespaceTable}].[{EntityName}])
		SET @FullScan = 1;
	END
	ELSE
	BEGIN	
		SET @FullScan = 0;

		--OK, build the entity collection. We combine the bit positions 
		--and only include the records where they match the bit position solutions passed
		--through from the front-end.
		;WITH Entities(Id, Score)AS
		(
			SELECT u.Id, SUM(u.Position)
			FROM OPENJSON(@Body, N'lax $.ParamsFilter.Params') F
			CROSS APPLY [{NamespaceTable}].[udf{EntityName}FilterProperty] (F.value) u
			GROUP BY u.Id
		)
		INSERT INTO [{NamespaceTable}].[{EntityName}SearchHistoryCache]
		SELECT @CollectionId AS [SearchId], E.Id AS [EntityId] 
		FROM Entities E
		INNER JOIN OPENJSON(@Body, N'lax $.ParamsFilter.Solutions') V ON V.value = E.Score;

		SET @RecordResult = ROWCOUNT_BIG();
	END

	--Set the record count in the collection.
	UPDATE [{NamespaceTable}].[{EntityName}SearchHistory]
		SET [RecordCount] = @RecordResult, [FullScan] = @FullScan
	WHERE [Id] = @CollectionId;

	RETURN 200;
END