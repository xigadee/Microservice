--#region.extension
CREATE PROCEDURE [{NamespaceMigration}].[{EntityName}_Extension_ManuallyPopulate]
AS
BEGIN

	DECLARE @Results TABLE   
	(  
		Id BIGINT PRIMARY KEY NOT NULL,
		ToUpdate BIT NOT NULL
	)

	;WITH ToUpdate(Id) AS
	(
		SELECT Id FROM [{NamespaceTable}].[{EntityName}] 
		INTERSECT
		SELECT EntityId FROM [{NamespaceTable}].[{EntityName}_Extension] 
	)
	INSERT @Results
	SELECT Id, 1 FROM ToUpdate;
	PRINT 'To update: ' + CAST(@@RowCount AS VARCHAR(50));


	;WITH ToInsert(Id) AS
	(
		SELECT Id FROM [{NamespaceTable}].[{EntityName}] 
		EXCEPT
		SELECT EntityId FROM [{NamespaceTable}].[{EntityName}_Extension] 
	)
	INSERT @Results
	SELECT Id, 0 FROM ToInsert;
	PRINT 'To insert: ' + CAST(@@RowCount AS VARCHAR(50));


	DECLARE @ItemId BIGINT, @ToUpdate BIT, @Body NVARCHAR(MAX)

	WHILE (1=1) 
	BEGIN
		SET @ItemId = NULL;

		SELECT TOP 1 @ItemId = R.Id, @ToUpdate = R.ToUpdate, @Body = E.Body 
		FROM @Results AS R
		INNER JOIN [{NamespaceTable}].[{EntityName}] AS E ON E.Id = R.Id;

		if (@ItemId IS NULL)
			BREAK;

		BEGIN TRY

			PRINT 'Executing: '+ CAST(@ItemId AS VARCHAR(50));

			EXEC [{NamespaceTable}].[{spUpsertRP}_Extension] @ItemId, @ToUpdate, @Body;

		END TRY
		BEGIN CATCH
			PRINT 'Error executing: ' + CAST(@ItemId AS VARCHAR(50)) 
				+ ' => ' + CAST(ERROR_NUMBER()AS VARCHAR(20)) 
				+ ' ' + ERROR_MESSAGE();
		END CATCH
		BEGIN TRY

			--We want to make sure that we remove from the results temporary table, otherwise we could 
			--get stuck in a loop.
			DELETE FROM @Results WHERE Id = @ItemId;
			IF (@@ROWCOUNT = 0)
			BEGIN
				PRINT 'Could not remove: ' + CAST(@ItemId AS VARCHAR(50));
				BREAK;
			END
		END TRY
		BEGIN CATCH
			PRINT 'Error removing from Temp table: ' + CAST(@ItemId AS VARCHAR(50)) 
				+ ' => ' + CAST(ERROR_NUMBER()AS VARCHAR(20)) 
				+ ' ' + ERROR_MESSAGE();
			BREAK;
		END CATCH
	END;
	
END
GO
--#endregion