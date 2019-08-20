--#region.extension
CREATE TABLE [dbo].[Test1_Extension]
(
     [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
     [EntityId] BIGINT NOT NULL,
	 [AccountId] UNIQUEIDENTIFIER NULL,
	 [Second] INT NULL,
	 CONSTRAINT [FK_Test1_Extension_EntityId] FOREIGN KEY ([EntityId]) REFERENCES [dbo].[Test1]([Id]), 
)
GO
CREATE UNIQUE INDEX[IX_Test1_Extension_EntityId] ON [dbo].[Test1_Extension] ([EntityId])
GO
--#endregion
GO
--#region.extension
CREATE VIEW [dbo].[ViewTest1]
AS
	SELECT E.*, EX.AccountId, EX.[Second]
	FROM [dbo].[Test1] AS E 
	INNER JOIN [dbo].[Test1_Extension] AS EX ON E.Id = EX.EntityId
GO
--#endregion
GO
--#region.extension
CREATE PROCEDURE [dbo].[spTest1UpsertRP_Extension]
	 @EntityId BIGINT
	,@Update BIT
	,@Body NVARCHAR (MAX)
AS
BEGIN

	DECLARE @AccountId UNIQUEIDENTIFIER = CAST(JSON_VALUE(@Body,'lax $.AccountId') AS UNIQUEIDENTIFIER);
	DECLARE @Second INT = CAST(JSON_VALUE(@Body,'lax $.Second') AS INT);

	IF (@Update = 0)
	BEGIN
		-- Insert record into DB and get its identity
		INSERT INTO [dbo].[Test1_Extension] 
		(
				EntityId,
				AccountId,
				[Second]
		)
		VALUES 
		(
				@EntityId,
				@AccountId,
				@Second
		)

		RETURN 200;
	END
	ELSE
	BEGIN
		UPDATE [dbo].[Test1_Extension] 
			SET [AccountId]=@AccountId, 
				[Second]=@Second
		WHERE EntityId = @EntityId;

		RETURN 200;
	END
	
	RETURN 400;
END
GO
--#endregion
GO
--#region.extension
CREATE PROCEDURE [dbo].[Test1_Extension_ManuallyPopulate]
AS
BEGIN

	DECLARE @Results TABLE   
	(  
		Id BIGINT PRIMARY KEY NOT NULL,
		ToUpdate BIT NOT NULL
	)

	;WITH ToUpdate(Id) AS
	(
		SELECT Id FROM [dbo].[Test1] 
		INTERSECT
		SELECT EntityId FROM [dbo].[Test1_Extension] 
	)
	INSERT @Results
	SELECT Id, 1 FROM ToUpdate;
	PRINT @@RowCount;


	;WITH ToInsert(Id) AS
	(
		SELECT Id FROM [dbo].[Test1] 
		EXCEPT
		SELECT EntityId FROM [dbo].[Test1_Extension] 
	)
	INSERT @Results
	SELECT Id, 0 FROM ToInsert;
	PRINT @@RowCount;


	DECLARE @ItemId BIGINT, @ToUpdate BIT, @Body NVARCHAR(MAX)

	WHILE (1=1) 
	BEGIN
		SELECT TOP 1 @ItemId = R.Id, @ToUpdate = R.ToUpdate, @Body = E.Body 
		FROM @Results AS R
		INNER JOIN [dbo].[Test1] AS E ON E.Id = R.Id

		IF (@@RowCount = 0)
			BREAK;	

		PRINT @ItemId;

		if (@ItemId IS NULL)
			BREAK;

		EXEC [dbo].[spTest1UpsertRP_Extension] @ItemId, @ToUpdate, @Body;

		DELETE FROM @Results WHERE ID = @ItemId;
	END;
	
END
GO
--#endregion
GO
