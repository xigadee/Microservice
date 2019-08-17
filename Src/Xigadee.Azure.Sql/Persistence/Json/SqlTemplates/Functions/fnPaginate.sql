CREATE FUNCTION [{NamespaceTable}].[udfPaginate{EntityName}Property] (@Body AS NVARCHAR(MAX))  
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
	DECLARE @IsDescending BIT = 0;
	--Rank
	IF (@Order IS NOT NULL)
	BEGIN
		DECLARE @IsDateField BIT = ISNULL(CAST(JSON_VALUE(@Order,'lax $.IsDateFieldParameter') AS BIT), 0);

		SET @IsDescending = ISNULL(CAST(JSON_VALUE(@Order,'lax $.IsDescending') AS BIT), 0);
		DECLARE @OrderParameter VARCHAR(50) = LOWER(CAST(JSON_VALUE(@Order,'lax $.Parameter') AS VARCHAR(50)));

		IF (@IsDateField = 1)
		BEGIN
			;WITH R1(Id,Score)AS
			(
				SELECT [Id]
				, CASE @OrderParameter 
					WHEN 'datecreated' THEN RANK() OVER(ORDER BY [DateCreated]) 
					WHEN 'dateupdated' THEN RANK() OVER(ORDER BY [DateUpdated]) 
					WHEN 'datecombined' THEN RANK() OVER(ORDER BY ISNULL([DateUpdated],[DateCreated])) 
				END
				FROM [{NamespaceTable}].[{EntityName}]
			)
			UPDATE @FilterIds
				SET [Rank] = R1.[Score]
			FROM @FilterIds f
			INNER JOIN R1 ON R1.Id = f.Id
		END
	END

RETURN;
END;  