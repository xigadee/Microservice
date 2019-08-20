--#region.extension
CREATE PROCEDURE [{NamespaceTable}].[{EntityName}_Extension_ManuallyPopulate]
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
	PRINT @@RowCount;


	;WITH ToInsert(Id) AS
	(
		SELECT Id FROM [{NamespaceTable}].[{EntityName}] 
		EXCEPT
		SELECT EntityId FROM [{NamespaceTable}].[{EntityName}_Extension] 
	)
	INSERT @Results
	SELECT Id, 0 FROM ToInsert;
	PRINT @@RowCount;


	DECLARE @ItemId BIGINT, @ToUpdate BIT, @Body NVARCHAR(MAX)

	WHILE (1=1) 
	BEGIN
		SELECT TOP 1 @ItemId = R.Id, @ToUpdate = R.ToUpdate, @Body = E.Body 
		FROM @Results AS R
		INNER JOIN [{NamespaceTable}].[{EntityName}] AS E ON E.Id = R.Id

		PRINT @ItemId;

		if (@ItemId IS NULL)
			BREAK;

		EXEC [{NamespaceTable}].[{spUpsertRP}_Extension] @ItemId, @ToUpdate, @Body;

		DELETE FROM @Results WHERE ID = @ItemId;
	END;
	
END
GO
--#endregion