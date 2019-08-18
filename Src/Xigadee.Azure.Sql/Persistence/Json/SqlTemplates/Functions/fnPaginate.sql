CREATE FUNCTION [{NamespaceTable}].[udf{EntityName}PaginateProperty] (@CollectionId BIGINT, @Body AS NVARCHAR(MAX))  
RETURNS @Results TABLE   
(  
    Id BIGINT PRIMARY KEY NOT NULL,
    Position INT NOT NULL
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
		DECLARE @OrderParameter VARCHAR(50) = LOWER(CAST(JSON_VALUE(@Order,'lax $.Parameter') AS VARCHAR(50)));

		INSERT INTO @Results
		SELECT C.[Id]
		, CASE @IsDateField
			WHEN 1 THEN
				CASE @OrderParameter 
					WHEN 'datecreated' THEN RANK() OVER(ORDER BY [E.DateCreated]) 
					WHEN 'dateupdated' THEN RANK() OVER(ORDER BY [E.DateUpdated]) 
					WHEN 'datecombined' THEN RANK() OVER(ORDER BY ISNULL([E.DateUpdated],[E.DateCreated]))
					ELSE 0
				END
			ELSE RANK() OVER(ORDER BY P.[Value]) 
			END
		FROM [{NamespaceTable}].[{EntityName}SearchHistoryCache] AS C
		INNER JOIN [{NamespaceTable}].[{EntityName}] AS E ON E.Id = C.EntityId
		LEFT JOIN [{NamespaceTable}].[{EntityName}Property] AS P ON @IsDateField = 0 AND P.EntityId = C.[EntityId]
		INNER JOIN [{NamespaceTable}].[{EntityName}PropertyKey] AS PK ON PK.[Type]=@OrderParameter AND P.[KeyId] = PK.[Id]
		WHERE C.[SearchId] = @CollectionId;

		RETURN;
	END
END