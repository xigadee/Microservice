--#region.extension
CREATE PROCEDURE [dbo].[spAccountUpsertRP_Extension]
	 @EntityId BIGINT
	,@Update BIT
	,@Body NVARCHAR (MAX)
AS
BEGIN
		IF (@Update = 0)
		BEGIN
			-- Insert record into DB and get its identity
			INSERT INTO [dbo].[Account_Extension] 
			(
				  EntityId
			)
			VALUES 
			(
				  @EntityId
			)

			RETURN 200;
		END
		ELSE
		BEGIN
			--UPDATE [dbo].[Account_Extension] 
			--	SET Something='';
			--WHERE EntityId = @EntityId
			RETURN 200;
		END
	
END
GO
--#endregion
GO
--#region.extension
CREATE PROCEDURE [dbo].[Account_Extension_ManuallyPopulate]
AS
BEGIN

	DECLARE @Results TABLE   
	(  
		Id BIGINT PRIMARY KEY NOT NULL,
		ToUpdate BIT NOT NULL
	)

	;WITH ToUpdate(Id) AS
	(
		SELECT Id FROM [dbo].[Account] 
		INTERSECT
		SELECT EntityId FROM [dbo].[Account_Extension] 
	)
	INSERT @Results
	SELECT Id, 1 FROM ToUpdate;
	PRINT 'To update: ' + CAST(@@RowCount AS VARCHAR(50));


	;WITH ToInsert(Id) AS
	(
		SELECT Id FROM [dbo].[Account] 
		EXCEPT
		SELECT EntityId FROM [dbo].[Account_Extension] 
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
		INNER JOIN [dbo].[Account] AS E ON E.Id = R.Id;

		if (@ItemId IS NULL)
			BREAK;

		BEGIN TRY

			PRINT 'Executing: '+ CAST(@ItemId AS VARCHAR(50));

			EXEC [dbo].[spAccountUpsertRP_Extension] @ItemId, @ToUpdate, @Body;

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
GO
