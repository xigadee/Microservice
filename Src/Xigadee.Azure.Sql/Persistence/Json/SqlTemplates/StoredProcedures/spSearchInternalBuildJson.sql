CREATE PROCEDURE [{NamespaceTable}].[{spSearch}InternalBuild_Json]
	@Body NVARCHAR(MAX),
	@ETag UNIQUEIDENTIFIER OUTPUT,
	@CollectionId BIGINT OUTPUT,
	@RecordResult BIGINT OUTPUT
AS
BEGIN
	DECLARE @HistoryIndexId BIGINT, @TimeStamp DATETIME
	SET @ETag = ISNULL(TRY_CONVERT(UNIQUEIDENTIFIER, JSON_VALUE(@Body,'lax $.ETag')), NEWID());

	--OK, check whether the ETag is already assigned to a results set
	SELECT TOP 1 @CollectionId = Id, @HistoryIndexId = [HistoryIndex], @TimeStamp = [TimeStamp]
	FROM [{NamespaceTable}].[{EntityName}SearchHistory] 
	WHERE ETag = @ETag;

	--OK, we need to check that the collection is still valid.
	DECLARE @CurrentHistoryIndexId BIGINT = (SELECT TOP 1 Id FROM [{NamespaceTable}].[{EntityName}History] ORDER BY Id DESC);

	IF (@CollectionId IS NOT NULL 
		AND @CurrentHistoryIndexId IS NOT NULL
		AND @HistoryIndexId IS NOT NULL
		AND @HistoryIndexId = @CurrentHistoryIndexId)
	BEGIN
		RETURN 202;
	END
	ELSE
	BEGIN
		--We need to create a new search collection and change the ETag.
		SET @ETag = NEWID();
	END

	INSERT INTO [{NamespaceTable}].[{EntityName}SearchHistory]
	([ETag],[EntityType],[SearchType],[Sig],[Body],[HistoryIndex])
	VALUES
	(@ETag, '{EntityName}', '{spSearch}Entity_Json', '', @Body, @CurrentHistoryIndexId);

	SET @CollectionId = @@IDENTITY;
	
	--OK, build the entity collection. We combine the bit positions 
	--and only include the records where they match the bit position solutions passed
	--through from the front-end.
	;WITH Entities(Id, Score)AS
	(
		SELECT u.Id,SUM(u.Position)
		FROM OPENJSON(@Body, N'lax $.Filters.Params') F
		CROSS APPLY [{NamespaceTable}].[udfFilter{EntityName}Property] (F.value) u
		GROUP BY u.Id
	)
	INSERT INTO [{NamespaceTable}].[{EntityName}SearchHistoryCache]
	VALUES(SearchId, EntityId)
	SELECT @CollectionId, E.Id FROM Entities E
	INNER JOIN OPENJSON(@Body, N'lax $.Filters.Solutions') V ON V.value = E.Score;

	SET @RecordResult = ROWCOUNT_BIG();

	RETURN 200;
END