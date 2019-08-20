CREATE FUNCTION [{NamespaceTable}].[udf{EntityName}PaginateProperty] (@FullScan BIT, @CollectionId BIGINT, @Body AS NVARCHAR(MAX))  
RETURNS @Results TABLE   
(  
    Id BIGINT PRIMARY KEY NOT NULL,
    [Rank] INT NOT NULL
)
AS  
BEGIN
	DECLARE @Skip INT = ISNULL(CAST(JSON_VALUE(@Body,'lax $.SkipValue') AS INT), 0);
	DECLARE @Top INT = ISNULL(CAST(JSON_VALUE(@Body,'lax $.TopValue') AS INT), 50);

	--Set the default parameters;
	DECLARE @IsDateField BIT = 1;
	DECLARE @IsDescending BIT = 1;
	DECLARE @OrderParameter VARCHAR(50) = 'datecreated';

	--Rank
	DECLARE @Order NVARCHAR(MAX) = (SELECT TOP 1 value FROM OPENJSON(@Body, N'lax $.ParamsOrderBy'))
	IF (@Order IS NOT NULL)
	BEGIN
		SET @IsDateField = ISNULL(CAST(JSON_VALUE(@Order,'lax $.IsDateFieldParameter') AS BIT), 0);
		SET @IsDescending = ISNULL(CAST(JSON_VALUE(@Order,'lax $.IsDescending') AS BIT), 0);
		SET @OrderParameter = LOWER(CAST(JSON_VALUE(@Order,'lax $.Parameter') AS VARCHAR(50)));
	END;

	;WITH Result (Id,[Rank]) AS
	(
		SELECT E.[Id]
		, CASE @IsDateField
			WHEN 1 THEN
				CASE @OrderParameter 
					WHEN 'datecreated' THEN 
						CASE @IsDescending WHEN 1 THEN RANK() OVER(ORDER BY E.[DateCreated] DESC) ELSE RANK() OVER(ORDER BY E.[DateCreated]) END
					WHEN 'dateupdated' THEN 
						CASE @IsDescending WHEN 1 THEN RANK() OVER(ORDER BY E.[DateUpdated] DESC) ELSE RANK() OVER(ORDER BY E.[DateUpdated]) END
					WHEN 'datecombined' THEN 
						CASE @IsDescending WHEN 1 THEN RANK() OVER(ORDER BY ISNULL(E.[DateUpdated],E.[DateCreated]) DESC) ELSE RANK() OVER(ORDER BY ISNULL(E.[DateUpdated],E.[DateCreated])) END
					ELSE 0
				END
			ELSE CASE @IsDescending WHEN 1 THEN RANK() OVER(ORDER BY P.[Value] DESC) ELSE RANK() OVER(ORDER BY P.[Value]) END
			END AS [Rank]
		FROM [{NamespaceTable}].[{EntityName}] AS E
		LEFT JOIN [{NamespaceTable}].[{EntityName}SearchHistoryCache] AS C ON E.Id = C.EntityId AND C.[SearchId] = @CollectionId
		LEFT JOIN [{NamespaceTable}].[{EntityName}Property] AS P ON @IsDateField = 0 AND P.EntityId = C.[EntityId]
		LEFT JOIN [{NamespaceTable}].[{EntityName}PropertyKey] AS PK ON PK.[Type]=@OrderParameter AND P.[KeyId] = PK.[Id]
		WHERE @FullScan=1 OR C.[Id] IS NOT NULL
	)
	INSERT INTO @Results
	SELECT Id, [Rank] FROM Result
	ORDER BY [Rank]
	OFFSET @Skip ROWS
	FETCH NEXT @Top ROWS ONLY
	
	RETURN;
END