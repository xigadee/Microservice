CREATE PROCEDURE [{NamespaceExternal}].[{spSearch}Entity_Default_Json]
	@Body NVARCHAR(MAX)
AS
BEGIN
	BEGIN TRY


		--Build
		DECLARE @FilterIds TABLE
		(
			Id BIGINT PRIMARY KEY NOT NULL,
			[Rank] INT
		);

		DECLARE @ETag UNIQUEIDENTIFIER;
		SET @ETag = ISNULL(TRY_CONVERT(UNIQUEIDENTIFIER, JSON_VALUE(@Body,'lax $.ETag')), NEWID());

		DECLARE @Skip INT = ISNULL(CAST(JSON_VALUE(@Body,'lax $.SkipValue') AS INT), 0);
		DECLARE @Top INT = ISNULL(CAST(JSON_VALUE(@Body,'lax $.TopValue') AS INT), 50);

		EXEC [{NamespaceTable}].spSearchLog @ETag, '{EntityName}', '{spSearch}Entity_Json', @Body;
	
		;WITH Entities(Id, Score)AS
		(
			SELECT u.Id,SUM(u.Position)
			FROM OPENJSON(@Body, N'lax $.Filters.Params') F
			CROSS APPLY [{NamespaceTable}].[udfFilter{EntityName}Property] (F.value) u
			GROUP BY u.Id
		)
		INSERT INTO @FilterIds
		SELECT E.Id,0 FROM Entities E
		INNER JOIN OPENJSON(@Body, N'lax $.Filters.Validity') V ON V.value = E.Score;

		DECLARE @Order NVARCHAR(MAX) = (SELECT TOP 1 value FROM OPENJSON(@Body, N'lax $.ParamsOrderBy'))
		DECLARE @IsDescending BIT = 0;
		--Rank
		IF (@Order IS NOT NULL)
		BEGIN
			DECLARE @IsDateField BIT = ISNULL(CAST(JSON_VALUE(@Order,'lax $.IsDateField') AS BIT), 0);
			SET @IsDescending = ISNULL(CAST(JSON_VALUE(@Order,'lax $.IsDescending') AS BIT), 0);
			DECLARE @OrderParameter VARCHAR(50) = LOWER(CAST(JSON_VALUE(@Order,'lax $.Parameter') AS VARCHAR(50)));

			IF (@IsDateField = 1)
			BEGIN
				;WITH R1(Id,Score)AS
				(
				  SELECT [Id]
				  , CASE @OrderParameter 
						WHEN 'datecreated' THEN RANK() OVER(ORDER BY [DateCreated]) 
						WHEN 'dateupdated' THEN RANK() OVER(ORDER BY ISNULL([DateUpdated],[DateCreated])) 
					END
				  FROM [{NamespaceTable}].[{EntityName}]
				)
				UPDATE @FilterIds
					SET [Rank] = R1.[Score]
				FROM @FilterIds f
				INNER JOIN R1 ON R1.Id = f.Id
			END
		END

		--Output
		IF (@IsDescending = 0)
		BEGIN
			--Output
			SELECT E.ExternalId, E.VersionId, E.Body 
			FROM @FilterIds AS F
			INNER JOIN [{NamespaceTable}].[{EntityName}] AS E ON F.Id = E.Id
			ORDER BY F.[Rank]
			OFFSET @Skip ROWS
			FETCH NEXT @Top ROWS ONLY
		END
		ELSE
		BEGIN
			--Output
			SELECT E.ExternalId, E.VersionId, E.Body 
			FROM @FilterIds AS F
			INNER JOIN [{NamespaceTable}].[{EntityName}] AS E ON F.Id = E.Id
			ORDER BY F.[Rank] DESC
			OFFSET @Skip ROWS
			FETCH NEXT @Top ROWS ONLY
		END
	
		RETURN 200;
	END TRY
	BEGIN CATCH
		SELECT ERROR_NUMBER() AS ErrorNumber, ERROR_MESSAGE() AS ErrorMessage; 
		RETURN 500;
	END CATCH
END