CREATE FUNCTION [{NamespaceTable}].[udf{EntityName}PaginateProperty] (@CollectionId BIGINT, @Body AS NVARCHAR(MAX))  
RETURNS @Results TABLE   
(  
    Id BIGINT PRIMARY KEY NOT NULL,
    [Rank] INT NOT NULL
)
--Returns a result set that lists all the employees who report to the   
--specific employee directly or indirectly.*/  
AS  
BEGIN

	DECLARE @Order NVARCHAR(MAX) = (SELECT TOP 1 value FROM OPENJSON(@Body, N'lax $.ParamsOrderBy'))

	--Rank
	IF (@Order IS NOT NULL)
	BEGIN
		DECLARE @IsDateField BIT = ISNULL(CAST(JSON_VALUE(@Order,'lax $.IsDateFieldParameter') AS BIT), 0);
		DECLARE @IsDescending BIT = ISNULL(CAST(JSON_VALUE(@Order,'lax $.IsDescending') AS BIT), 0);
		DECLARE @OrderParameter VARCHAR(50) = LOWER(CAST(JSON_VALUE(@Order,'lax $.Parameter') AS VARCHAR(50)));

		INSERT INTO @Results
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
		FROM [{NamespaceTable}].[{EntityName}SearchHistoryCache] AS C
		INNER JOIN [{NamespaceTable}].[{EntityName}] AS E ON E.Id = C.EntityId
		LEFT JOIN [{NamespaceTable}].[{EntityName}Property] AS P ON @IsDateField = 0 AND P.EntityId = C.[EntityId]
		LEFT JOIN [{NamespaceTable}].[{EntityName}PropertyKey] AS PK ON PK.[Type]=@OrderParameter AND P.[KeyId] = PK.[Id]
		WHERE C.[SearchId] = @CollectionId;

	END

	RETURN;
END