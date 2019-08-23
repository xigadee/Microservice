CREATE TABLE [dbo].[AccountReferenceKey]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [Type] VARCHAR(20) NULL
)
GO

CREATE UNIQUE INDEX [IX_AccountReferenceKey_Type] ON [dbo].[AccountReferenceKey] ([Type])
GO
CREATE TABLE [dbo].[AccountPropertyKey]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [Type] VARCHAR(30) NULL
)
GO

CREATE UNIQUE INDEX [IX_AccountPropertyKey_Type] ON [dbo].[AccountPropertyKey] ([Type])
GO


CREATE TABLE [dbo].[Account]
(
     [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1)
	,[ExternalId] UNIQUEIDENTIFIER NOT NULL
    ,[VersionId] UNIQUEIDENTIFIER NULL 
    ,[UserIdAudit] UNIQUEIDENTIFIER NULL 
	,[DateCreated] DATETIME NOT NULL DEFAULT(GETUTCDATE())
	,[DateUpdated] DATETIME NULL
	,[Sig] VARCHAR(256) NULL
	,[Body] NVARCHAR(MAX)
)
GO
CREATE UNIQUE INDEX[IX_Account_ExternalId] ON [dbo].[Account] ([ExternalId]) INCLUDE ([VersionId])

GO
CREATE TABLE[dbo].[AccountHistory]
(
     [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1)
    ,[EntityId] BIGINT NOT NULL 
	,[ExternalId] UNIQUEIDENTIFIER NOT NULL
    ,[VersionId] UNIQUEIDENTIFIER NULL 
    ,[UserIdAudit] UNIQUEIDENTIFIER NULL 
	,[DateCreated] DATETIME NOT NULL 
	,[DateUpdated] DATETIME NULL
	,[TimeStamp] DATETIME NOT NULL DEFAULT(GETUTCDATE())
	,[Sig] VARCHAR(256) NULL
	,[Body] NVARCHAR(MAX) NULL
)

GO
CREATE TABLE [dbo].[AccountProperty]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), 
	[EntityId] BIGINT NOT NULL,
    [KeyId] INT NOT NULL, 
    [Value] NVARCHAR(250) NOT NULL, 
    CONSTRAINT [FK_AccountProperty_Id] FOREIGN KEY ([EntityId]) REFERENCES [dbo].[Account]([Id]), 
    CONSTRAINT [FK_AccountProperty_KeyId] FOREIGN KEY ([KeyId]) REFERENCES [dbo].[AccountPropertyKey]([Id])
)
GO
CREATE INDEX [IX_AccountProperty_KeyId] ON [dbo].[AccountProperty] ([KeyId]) INCLUDE ([EntityId])
GO
CREATE INDEX [IX_AccountProperty_EntityId] ON [dbo].[AccountProperty] ([EntityId]) INCLUDE ([Id])

GO
CREATE TABLE [dbo].[AccountReference]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), 
	[EntityId] BIGINT NOT NULL,
    [KeyId] INT NOT NULL, 
    [Value] NVARCHAR(250) NOT NULL, 
    CONSTRAINT [FK_AccountReference_Id] FOREIGN KEY ([EntityId]) REFERENCES [dbo].[Account]([Id]), 
    CONSTRAINT [FK_AccountReference_KeyId] FOREIGN KEY ([KeyId]) REFERENCES [dbo].[AccountReferenceKey]([Id])
)
GO
CREATE UNIQUE INDEX [IX_AccountReference_TypeReference] ON [dbo].[AccountReference] ([KeyId],[Value]) INCLUDE ([EntityId])
GO
CREATE INDEX [IX_AccountReference_EntityId] ON [dbo].[AccountReference] ([EntityId]) INCLUDE ([Id])
GO
CREATE INDEX [IX_AccountReference_KeyId] ON [dbo].[AccountReference] ([KeyId],[EntityId]) INCLUDE ([Value])

GO
CREATE TABLE [dbo].[AccountSearchHistory]
(
     [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1)
	,[ETag] UNIQUEIDENTIFIER NOT NULL
	,[EntityType] VARCHAR(50) NOT NULL
	,[SearchType] VARCHAR(50) NOT NULL
	,[TimeStamp] DATETIME NOT NULL DEFAULT(GETUTCDATE())
	,[Sig] VARCHAR(256) NULL
	,[Body] NVARCHAR(MAX) NULL
	,[HistoryIndex] BIGINT NULL
	,[RecordCount] BIGINT NOT NULL DEFAULT(-1)
	,[FullScan] BIT NULL
)
GO 
CREATE UNIQUE INDEX[IX_AccountSearchHistory_ETag] ON [dbo].[AccountSearchHistory] ([ETag]) INCLUDE([HistoryIndex])
GO
CREATE TABLE [dbo].[AccountSearchHistoryCache]
(
     [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1)
    ,[SearchId] BIGINT NOT NULL 
    ,[EntityId] BIGINT NOT NULL 
	,CONSTRAINT [FK_AccountSearchHistoryCache_SearchId] FOREIGN KEY ([SearchId]) REFERENCES [dbo].[AccountSearchHistory]([Id])
	,CONSTRAINT [FK_AccountSearchHistoryCache_EntityId] FOREIGN KEY ([EntityId]) REFERENCES [dbo].[Account]([Id])
)
GO
CREATE INDEX[IX_AccountSearchHistoryCache_SearchHistory] ON [dbo].[AccountSearchHistoryCache] ([SearchId]) INCLUDE ([EntityId]) 
GO


GO
CREATE PROCEDURE [dbo].[spAccountUpsertRP]
	@EntityId BIGINT,
	@References [External].[KvpTableType] READONLY,
	@Properties [External].[KvpTableType] READONLY
AS
	BEGIN TRY

		--Process the reference first.
		INSERT INTO [dbo].[AccountReferenceKey] WITH (UPDLOCK) ([Type])
		SELECT DISTINCT [RefType] FROM @References
		EXCEPT
		SELECT [Type] FROM [dbo].[AccountReferenceKey]

		--Remove old records
		DELETE FROM [dbo].[AccountReference] WITH (UPDLOCK)
		WHERE [EntityId] = @EntityId

		--Add the new records.
		INSERT INTO [dbo].[AccountReference] WITH (UPDLOCK)
		(
		      [EntityId]
			, [KeyId]
			, [Value]
		)
		SELECT @EntityId, K.[Id], R.RefValue
		FROM @References AS R 
		INNER JOIN [dbo].[AccountReferenceKey] AS K ON R.[RefType] = K.[Type]
	
		--Now process the properties.
		INSERT INTO [dbo].[AccountPropertyKey] WITH (UPDLOCK) ([Type])
		SELECT DISTINCT [RefType] FROM @Properties
		EXCEPT
		SELECT [Type] FROM [dbo].[AccountPropertyKey]

		--Remove old records
		DELETE FROM [dbo].[AccountProperty] WITH (UPDLOCK)
		WHERE [EntityId] = @EntityId

		--Add the new records.
		INSERT INTO [dbo].[AccountProperty] WITH (UPDLOCK)
		(
			  [EntityId]
			, [KeyId]
			, [Value]
		)
		SELECT @EntityId, K.[Id], P.RefValue
		FROM @Properties AS P
		INNER JOIN [dbo].[AccountPropertyKey] AS K ON P.[RefType] = K.[Type]

		RETURN 200;
	END TRY
	BEGIN CATCH
		IF (ERROR_NUMBER() = 2601)
			RETURN 409; --Conflict - duplicate reference key to other transaction.
		ELSE
			THROW;
	END CATCH

GO
CREATE PROCEDURE [dbo].[spAccountHistory]
	 @EntityId BIGINT
	,@ExternalId UNIQUEIDENTIFIER
	,@VersionIdNew UNIQUEIDENTIFIER 
    ,@UserIdAudit UNIQUEIDENTIFIER 
	,@Body NVARCHAR (MAX)
	,@DateCreated DATETIME 
	,@DateUpdated DATETIME 
	,@Sig VARCHAR(256) 
AS
	BEGIN TRY

		--Process the reference first.
		INSERT INTO [dbo].[AccountHistory] 
		(
			 [EntityId]
			,[ExternalId] 
			,[VersionId] 
			,[UserIdAudit] 
			,[DateCreated] 
			,[DateUpdated] 
			,[Sig] 
			,[Body]
		)
		VALUES
		(
			 @EntityId 
			,@ExternalId 
			,@VersionIdNew  
			,@UserIdAudit  
			,@DateCreated  
			,@DateUpdated  
			,@Sig 
			,@Body 
		)

		RETURN 200;
	END TRY
	BEGIN CATCH
		IF (ERROR_NUMBER() = 2601)
			RETURN 409; --Conflict - duplicate reference key to other transaction.
		ELSE
			THROW;
	END CATCH

GO
CREATE PROCEDURE [External].[spAccountCreate]
	 @ExternalId UNIQUEIDENTIFIER
	,@VersionId UNIQUEIDENTIFIER = NULL
	,@VersionIdNew UNIQUEIDENTIFIER = NULL
    ,@UserIdAudit UNIQUEIDENTIFIER = NULL 
	,@Body NVARCHAR (MAX)
	,@DateCreated DATETIME = NULL
	,@DateUpdated DATETIME = NULL
	,@References [External].[KvpTableType] READONLY 
	,@Properties [External].[KvpTableType] READONLY 
	,@Sig VARCHAR(256) = NULL
AS
	BEGIN TRY
		BEGIN TRAN

		-- Insert record into DB and get its identity
		INSERT INTO [dbo].[Account] 
		(
			  ExternalId
			, VersionId
			, UserIdAudit
			, DateCreated
			, DateUpdated
			, Sig
			, Body
		)
		VALUES 
		(
			  @ExternalId
			, ISNULL(@VersionIdNew, NEWID())
			, @UserIdAudit
			, ISNULL(@DateCreated, GETUTCDATE())
			, @DateUpdated
			, @Sig
			, @Body
		)
		DECLARE @Id BIGINT = SCOPE_IDENTITY();
	
		-- Create references and properties
		DECLARE @RPResponse INT;
		EXEC @RPResponse = [dbo].[spAccountUpsertRP] @Id, @References, @Properties
		IF (@RPResponse = 409)
		BEGIN
			ROLLBACK TRAN;
			RETURN 409; --Conflict with other entities.
		END

		--#region.extension
		DECLARE @ExResponse INT;
		EXEC @ExResponse = [dbo].[spAccountUpsertRP_Extension] @Id, 0, @Body
		IF (@ExResponse != 200)
		BEGIN
			ROLLBACK TRAN;
			RETURN 400; --Conflict with extension table - this should not happen.
		END
		--#endregion

		--Record the audit history.
		EXEC [dbo].[spAccountHistory] @Id, @ExternalId, @VersionIdNew, @UserIdAudit, @Body, @DateCreated, @DateUpdated, @Sig

		COMMIT TRAN;

		RETURN 201;
	END TRY
	BEGIN CATCH
		SELECT  ERROR_NUMBER() AS ErrorNumber, ERROR_MESSAGE() AS ErrorMessage; 
		ROLLBACK TRAN;
		RETURN 500;
	END CATCH

GO
CREATE PROCEDURE [External].[spAccountDelete]
	@ExternalId UNIQUEIDENTIFIER
AS
SET NOCOUNT ON;
	BEGIN TRY
	
		BEGIN TRAN

		DECLARE @Id BIGINT = (SELECT [Id] FROM [dbo].[Account] WHERE [ExternalId] = @ExternalId)

		IF (@Id IS NULL)
		BEGIN
			ROLLBACK TRAN
			RETURN 404;
		END

		--#region.extension
		DELETE FROM [dbo].[Account_Extension]
		WHERE [EntityId] = @Id
		--#endregion

		DELETE FROM [dbo].[AccountProperty]
		WHERE [EntityId] = @Id

		DELETE FROM [dbo].[AccountReference]
		WHERE [EntityId] = @Id

		DELETE FROM [dbo].[Account]
		WHERE [Id] = @Id

		--Not found.
		COMMIT TRAN

		RETURN 200;

	END TRY
	BEGIN CATCH
		SELECT  ERROR_NUMBER() AS ErrorNumber, ERROR_MESSAGE() AS ErrorMessage;
		ROLLBACK TRAN;
		RETURN 500;
	END CATCH
GO
CREATE PROCEDURE [External].[spAccountDeleteByRef]
	@RefType VARCHAR(30),
	@RefValue NVARCHAR(250) 
AS
SET NOCOUNT ON;
	BEGIN TRY
		
		BEGIN TRAN

		DECLARE @Id BIGINT = (
			SELECT E.[Id] FROM [dbo].[Account] E
			INNER JOIN [dbo].[AccountReference] R ON E.Id = R.EntityId
			INNER JOIN [dbo].[AccountReferenceKey] RK ON R.KeyId = RK.Id
			WHERE RK.[Type] = @RefType AND R.[Value] = @RefValue
		)

		IF (@Id IS NULL)
		BEGIN
			ROLLBACK TRAN
			RETURN 404;
		END

		--#region.extension
		DELETE FROM [dbo].[Account_Extension]
		WHERE [EntityId] = @Id
		--#endregion

		DELETE FROM [dbo].[AccountProperty]
		WHERE [EntityId] = @Id

		DELETE FROM [dbo].[AccountReference]
		WHERE [EntityId] = @Id

		DELETE FROM [dbo].[Account]
		WHERE [Id] = @Id

		--Not found.
		COMMIT TRAN

		RETURN 200;

	END TRY
	BEGIN CATCH
		SELECT  ERROR_NUMBER() AS ErrorNumber, ERROR_MESSAGE() AS ErrorMessage;
		ROLLBACK TRAN
		RETURN 500
	END CATCH
GO
CREATE PROCEDURE [External].[spAccountRead]
	@ExternalId UNIQUEIDENTIFIER
AS
SET NOCOUNT ON;
	BEGIN TRY
	
		SELECT [Sig],[Body]
		FROM [dbo].[Account]
		WHERE [ExternalId] = @ExternalId

		IF (@@ROWCOUNT>0)
			RETURN 200;
	
		--Not found.
		RETURN 404;

	END TRY
	BEGIN CATCH
		SELECT  ERROR_NUMBER() AS ErrorNumber, ERROR_MESSAGE() AS ErrorMessage;
		RETURN 500
	END CATCH
GO
CREATE PROCEDURE [External].[spAccountReadByRef]
	@RefType VARCHAR(30),
	@RefValue NVARCHAR(250) 
AS
SET NOCOUNT ON;
	BEGIN TRY
	
		SELECT E.[Sig],E.[Body] 
		FROM [dbo].[Account] E
		INNER JOIN [dbo].[AccountReference] R ON E.Id = R.EntityId
		INNER JOIN [dbo].[AccountReferenceKey] RK ON R.KeyId = RK.Id
		WHERE RK.[Type] = @RefType AND R.[Value] = @RefValue

		IF (@@ROWCOUNT>0)
			RETURN 200;
	
		--Not found.
		RETURN 404;

	END TRY
	BEGIN CATCH
		SELECT  ERROR_NUMBER() AS ErrorNumber, ERROR_MESSAGE() AS ErrorMessage;
		RETURN 500
	END CATCH
GO
CREATE PROCEDURE [External].[spAccountUpdate]
	 @ExternalId UNIQUEIDENTIFIER
	,@VersionId UNIQUEIDENTIFIER = NULL
	,@VersionIdNew UNIQUEIDENTIFIER = NULL
    ,@UserIdAudit UNIQUEIDENTIFIER = NULL 
	,@Body NVARCHAR (MAX)
	,@DateCreated DATETIME = NULL
	,@DateUpdated DATETIME = NULL
	,@References [External].[KvpTableType] READONLY
	,@Properties [External].[KvpTableType] READONLY
	,@Sig VARCHAR(256) = NULL
AS
	BEGIN TRY
		BEGIN TRAN

		DECLARE @Id BIGINT

		IF (@VersionId IS NOT NULL)
		BEGIN
			DECLARE @ExistingVersion UNIQUEIDENTIFIER;
			SELECT @Id = [Id], @ExistingVersion = [VersionId] FROM [dbo].[Account] WHERE [ExternalId] = @ExternalId
			IF (@Id IS NOT NULL AND @ExistingVersion != @VersionId)
			BEGIN
				ROLLBACK TRAN;
				RETURN 409; --Conflict
			END
		END
		ELSE
		BEGIN
			SELECT @Id = [Id] FROM [dbo].[Account] WHERE [ExternalId] = @ExternalId
		END

		--Check we can find the entity
		IF (@Id IS NULL)
		BEGIN
			ROLLBACK TRAN;
			RETURN 404; --Not found
		END

		-- Insert record into DB and get its identity
		UPDATE [dbo].[Account]
		SET   [VersionId] = @VersionIdNew
			, [UserIdAudit] = @UserIdAudit
			, [DateUpdated] = ISNULL(@DateUpdated, GETUTCDATE())
			, [Sig] = @Sig
			, [Body] = @Body
		WHERE Id = @Id

		IF (@@ROWCOUNT=0)
		BEGIN
			ROLLBACK TRAN;
			RETURN 409; -- Update failure
		END
	
		-- Create references and properties
		DECLARE @RPResponse INT;
		EXEC @RPResponse = [dbo].[spAccountUpsertRP] @Id, @References, @Properties
		IF (@RPResponse = 409)
		BEGIN
			ROLLBACK TRAN;
			RETURN 409; --Not found
		END

		--#region.extension
		DECLARE @ExResponse INT;
		EXEC @ExResponse = [dbo].[spAccountUpsertRP_Extension] @Id, 1, @Body
		IF (@ExResponse != 200)
		BEGIN
			ROLLBACK TRAN;
			RETURN 400; --Conflict with extension table - this should not happen.
		END
		--#endregion

	    --Record the audit history.
		EXEC [dbo].[spAccountHistory] @Id, @ExternalId, @VersionIdNew, @UserIdAudit, @Body, @DateCreated, @DateUpdated, @Sig

		COMMIT TRAN;

		RETURN 200;
	END TRY
	BEGIN CATCH
		SELECT  ERROR_NUMBER() AS ErrorNumber, ERROR_MESSAGE() AS ErrorMessage; 
		ROLLBACK TRAN;

		RETURN 500;
	END CATCH

GO
CREATE PROCEDURE [External].[spAccountVersion]
	@ExternalId UNIQUEIDENTIFIER
AS
SET NOCOUNT ON;
	BEGIN TRY
	
		SELECT [ExternalId], [VersionId]
		FROM [dbo].[Account]
		WHERE [ExternalId] = @ExternalId

		IF (@@ROWCOUNT>0)
			RETURN 200;
	
		--Not found.
		RETURN 404;

	END TRY
	BEGIN CATCH
		SELECT  ERROR_NUMBER() AS ErrorNumber, ERROR_MESSAGE() AS ErrorMessage;
		RETURN 500
	END CATCH
GO
CREATE PROCEDURE [External].[spAccountVersionByRef]
	@RefType VARCHAR(30),
	@RefValue NVARCHAR(250) 
AS
SET NOCOUNT ON;
	BEGIN TRY
	
		SELECT E.[ExternalId], E.[VersionId]
		FROM [dbo].[Account] E
		INNER JOIN [dbo].[AccountReference] R ON E.Id = R.EntityId
		INNER JOIN [dbo].[AccountReferenceKey] RK ON R.KeyId = RK.Id
		WHERE RK.[Type] = @RefType AND R.[Value] = @RefValue

		IF (@@ROWCOUNT>0)
			RETURN 200;
	
		--Not found.
		RETURN 404;

	END TRY
	BEGIN CATCH
		SELECT  ERROR_NUMBER() AS ErrorNumber, ERROR_MESSAGE() AS ErrorMessage;
		RETURN 500
	END CATCH
GO
CREATE PROCEDURE [dbo].[spAccountSearchInternalBuild_Default]
	@PropertiesFilter [External].[KvpTableType] READONLY,
	@PropertyOrder [External].[KvpTableType] READONLY,
	@Skip INT = 0,
	@Top INT = 50
AS
BEGIN
	DECLARE @ParamCount INT = (SELECT COUNT(*) FROM @PropertiesFilter);

	IF (@Skip IS NULL)
		SET @Skip = 0;
	IF (@Top IS NULL)
		SET @Top = 50;

	IF (@ParamCount = 0)
		SELECT P.Id
		FROM [dbo].[Account] AS P
		ORDER BY P.Id
		OFFSET @Skip ROWS
		FETCH NEXT @Top ROWS ONLY
	ELSE IF (@ParamCount = 1)
		SELECT P.EntityId As Id
		FROM [dbo].[AccountProperty] AS P
		INNER JOIN [dbo].[AccountPropertyKey] PK ON P.KeyId = PK.Id
		INNER JOIN @PropertiesFilter PF ON PF.RefType = PK.[Type] AND PF.RefValue = P.Value
		ORDER BY P.EntityId
		OFFSET @Skip ROWS
		FETCH NEXT @Top ROWS ONLY
	ELSE
		SELECT R.Id
		FROM
		(
			SELECT P.EntityId As Id, 1 AS Num
			FROM [dbo].[AccountProperty] AS P
			INNER JOIN [dbo].[AccountPropertyKey] PK ON P.KeyId = PK.Id
			INNER JOIN @PropertiesFilter PF ON PF.RefType = PK.[Type] AND PF.RefValue = P.Value
		)AS R
		GROUP BY R.Id
		HAVING SUM(R.Num) = @ParamCount
		ORDER BY R.Id
		OFFSET @Skip ROWS
		FETCH NEXT @Top ROWS ONLY


END
GO
CREATE PROCEDURE [External].[spAccountSearch_Default]
	@ETag VARCHAR(50) = NULL,
	@PropertiesFilter [External].[KvpTableType] READONLY,
	@PropertyOrder [External].[KvpTableType] READONLY,
	@Skip INT = 0,
	@Top INT = 50
AS
BEGIN
	BEGIN TRY
		--Build
		DECLARE @FilterIds TABLE
		(
			Id BIGINT
		);

		INSERT INTO @FilterIds
			EXEC [dbo].[spAccountSearchInternalBuild_Default] @PropertiesFilter, @PropertyOrder, @Skip, @Top

		--Output
		SELECT E.ExternalId, E.VersionId, 
		(
			SELECT PK.[Type] AS 'Type',P.[Value] AS 'Value'
			FROM [dbo].[AccountProperty] AS P
			INNER JOIN [dbo].[AccountPropertyKey] PK ON P.KeyId = PK.Id
			WHERE F.Id = P.EntityId
			FOR JSON PATH, ROOT('Property')
		) AS Body
		FROM @FilterIds AS F
		INNER JOIN [dbo].[Account] AS E ON F.Id = E.Id;

		RETURN 200;
	END TRY
	BEGIN CATCH
		SELECT ERROR_NUMBER() AS ErrorNumber, ERROR_MESSAGE() AS ErrorMessage; 
		RETURN 500;
	END CATCH
END
GO
CREATE PROCEDURE [External].[spAccountSearchEntity_Default]
	@ETag VARCHAR(50) = NULL,
	@PropertiesFilter [External].[KvpTableType] READONLY,
	@PropertyOrder [External].[KvpTableType] READONLY,
	@Skip INT = 0,
	@Top INT = 50
AS
BEGIN
	BEGIN TRY
		--Build
		DECLARE @FilterIds TABLE
		(
			Id BIGINT
		);

		INSERT INTO @FilterIds
			EXEC [dbo].[spAccountSearchInternalBuild_Default] @PropertiesFilter, @PropertyOrder, @Skip, @Top

		--Output
		SELECT E.ExternalId, E.VersionId, E.Body 
		FROM @FilterIds AS F
		INNER JOIN [dbo].[Account] AS E ON F.Id = E.Id;

		RETURN 200;
	END TRY
	BEGIN CATCH
		SELECT ERROR_NUMBER() AS ErrorNumber, ERROR_MESSAGE() AS ErrorMessage; 
		RETURN 500;
	END CATCH
END
GO
CREATE PROCEDURE [dbo].[spAccountSearchInternalBuild_Json]
	@Body NVARCHAR(MAX),
	@ETag UNIQUEIDENTIFIER OUTPUT,
	@CollectionId BIGINT OUTPUT,
	@RecordResult BIGINT OUTPUT,
	@CacheHit INT OUTPUT,
	@FullScan BIT OUTPUT
AS
BEGIN
	DECLARE @HistoryIndexId BIGINT, @TimeStamp DATETIME
	SET @ETag = TRY_CONVERT(UNIQUEIDENTIFIER, JSON_VALUE(@Body,'lax $.ETag'));

	DECLARE @ParamsCount INT = ISNULL(TRY_CONVERT(INT, JSON_VALUE(@Body,'lax $.Filters.Count')),0);

	SET @CacheHit = 0;

	--OK, we need to check that the collection is still valid.
	DECLARE @CurrentHistoryIndexId BIGINT = (SELECT TOP 1 Id FROM [dbo].[AccountHistory] ORDER BY Id DESC);

	IF (@ParamsCount > 0 AND @ETag IS NOT NULL)
	BEGIN
		--OK, check whether the ETag is already assigned to a results set
		SELECT TOP 1 @CollectionId = Id, @HistoryIndexId = [HistoryIndex], @TimeStamp = [TimeStamp], @RecordResult = [RecordCount]
		FROM [dbo].[AccountSearchHistory] 
		WHERE ETag = @ETag;

		IF (@CollectionId IS NOT NULL 
			AND @CurrentHistoryIndexId IS NOT NULL
			AND @HistoryIndexId IS NOT NULL
			AND @HistoryIndexId = @CurrentHistoryIndexId)
		BEGIN
			RETURN 202;
		END
	END

	--We need to create a new search collection and change the ETag.
	SET @ETag = NEWID();

	INSERT INTO [dbo].[AccountSearchHistory]
	([ETag],[EntityType],[SearchType],[Sig],[Body],[HistoryIndex])
	VALUES
	(@ETag, 'Account', 'spAccountSearchInternalBuild_Json', '', @Body, @CurrentHistoryIndexId);

	SET @CollectionId = @@IDENTITY;

	IF (@ParamsCount = 0)
	BEGIN
		SET @RecordResult = (SELECT COUNT(*) FROM  [dbo].[Account])
		SET @FullScan = 1;
	END
	ELSE
	BEGIN	
		SET @FullScan = 0;

		--OK, build the entity collection. We combine the bit positions 
		--and only include the records where they match the bit position solutions passed
		--through from the front-end.
		;WITH Entities(Id, Score)AS
		(
			SELECT u.Id, SUM(u.Position)
			FROM OPENJSON(@Body, N'lax $.Filters.Params') F
			CROSS APPLY [dbo].[udfAccountFilterProperty] (F.value) u
			GROUP BY u.Id
		)
		INSERT INTO [dbo].[AccountSearchHistoryCache]
		SELECT @CollectionId AS [SearchId], E.Id AS [EntityId] 
		FROM Entities E
		INNER JOIN OPENJSON(@Body, N'lax $.Filters.Solutions') V ON V.value = E.Score;

		SET @RecordResult = ROWCOUNT_BIG();
	END

	--Set the record count in the collection.
	UPDATE [dbo].[AccountSearchHistory]
		SET [RecordCount] = @RecordResult, [FullScan] = @FullScan
	WHERE [Id] = @CollectionId;

	RETURN 200;
END
GO
CREATE PROCEDURE [External].[spAccountSearch_Default_Json]
	@Body NVARCHAR(MAX)
AS
BEGIN
	BEGIN TRY

	DECLARE @ETag UNIQUEIDENTIFIER, @CollectionId BIGINT, @Result INT, @RecordResult BIGINT, @CacheHit INT, @FullScan BIT

	EXEC @Result = [dbo].[spAccountSearchInternalBuild_Json] 
		@Body, @ETag OUTPUT, @CollectionId OUTPUT, @RecordResult OUTPUT, @CacheHit OUTPUT, @FullScan OUTPUT

	IF (@RecordResult = 0)
	BEGIN
		RETURN 202;
	END

	DECLARE @Skip INT = ISNULL(CAST(JSON_VALUE(@Body,'lax $.SkipValue') AS INT), 0);
	DECLARE @Top INT = ISNULL(CAST(JSON_VALUE(@Body,'lax $.TopValue') AS INT), 50);

	--Output
	SELECT E.ExternalId, E.VersionId, 
		(
			SELECT PK.[Type] AS 'Type',P.[Value] AS 'Value'
			FROM [dbo].[AccountProperty] AS P
			INNER JOIN [dbo].[AccountPropertyKey] PK ON P.KeyId = PK.Id
			WHERE F.Id = P.EntityId
			FOR JSON PATH, ROOT('Property')
		) AS Body
	FROM [dbo].[udfAccountPaginateProperty](@FullScan, @CollectionId, @Body) AS F
	INNER JOIN [dbo].[Account] AS E ON F.Id = E.Id
	ORDER BY F.[Rank]
	--OFFSET @Skip ROWS
	--FETCH NEXT @Top ROWS ONLY
	
	RETURN 200;

	END TRY
	BEGIN CATCH
		SELECT ERROR_NUMBER() AS ErrorNumber, ERROR_MESSAGE() AS ErrorMessage; 
		RETURN 500;
	END CATCH
END
GO
CREATE PROCEDURE [External].[spAccountSearchEntity_Default_Json]
	@Body NVARCHAR(MAX)
AS
BEGIN
	BEGIN TRY

	DECLARE @ETag UNIQUEIDENTIFIER, @CollectionId BIGINT, @Result INT, @RecordResult BIGINT, @CacheHit INT, @FullScan BIT

	EXEC @Result = [dbo].[spAccountSearchInternalBuild_Json] 
		@Body, @ETag OUTPUT, @CollectionId OUTPUT, @RecordResult OUTPUT, @CacheHit OUTPUT, @FullScan OUTPUT

	IF (@RecordResult = 0)
	BEGIN
		RETURN 202;
	END

	DECLARE @Skip INT = ISNULL(CAST(JSON_VALUE(@Body,'lax $.SkipValue') AS INT), 0);
	DECLARE @Top INT = ISNULL(CAST(JSON_VALUE(@Body,'lax $.TopValue') AS INT), 50);

	--Output
	SELECT '' AS [Sig], (
		SELECT @ETag AS [ETag], @RecordResult AS [RecordCount], 'Entity' AS [Type], @CacheHit AS [CacheHit]
		, @Skip AS [Skip], @Top AS [Top]
		,( 
			SELECT E.ExternalId AS [ExternalId], E.VersionId AS [VersionId], E.Body AS [Body]
			FROM [dbo].[udfAccountPaginateProperty](@FullScan, @CollectionId, @Body) AS F
			INNER JOIN [dbo].[Test1] AS E ON F.Id = E.Id
			ORDER BY F.[Rank]
			--OFFSET @Skip ROWS
			--FETCH NEXT @Top ROWS ONLY
			FOR JSON PATH
		) AS [Results]
		FOR JSON PATH, ROOT('SearchResponse')
	) AS [Body];

	RETURN 200;

	END TRY
	BEGIN CATCH
		SELECT ERROR_NUMBER() AS ErrorNumber, ERROR_MESSAGE() AS ErrorMessage; 
		RETURN 500;
	END CATCH

END
GO
CREATE FUNCTION [dbo].[udfAccountFilterProperty] (@Body AS NVARCHAR(MAX))  
RETURNS @Results TABLE   
(  
    Id BIGINT PRIMARY KEY NOT NULL,
    Position INT NOT NULL
)  
--Returns a result set that lists all the employees who report to the   
--specific employee directly or indirectly.*/  
AS  
BEGIN  

DECLARE @Parameter VARCHAR(30) = JSON_VALUE(@Body,'lax $.Parameter');
IF (@Parameter IS NULL)
	RETURN;

DECLARE @PropertyKey INT = (SELECT Id FROM [dbo].[AccountPropertyKey] WHERE [Type] = @Parameter);
IF (@PropertyKey IS NULL)
	RETURN;

DECLARE @Operator VARCHAR(30) = JSON_VALUE(@Body,'lax $.Operator');
IF (@Operator IS NULL)
	RETURN;

DECLARE @Position INT = TRY_CONVERT(INT, JSON_VALUE(@Body,'lax $.Position'));
IF (@Position IS NULL)
	RETURN;

DECLARE @OutputPosition INT = POWER(2, @Position);

DECLARE @Value NVARCHAR(250) = JSON_VALUE(@Body,'lax $.ValueRaw');
DECLARE @IsNullOperator BIT = TRY_CONVERT(BIT, JSON_VALUE(@Body,'lax $.IsNullOperator'));
DECLARE @IsEqual BIT = TRY_CONVERT(BIT, JSON_VALUE(@Body,'lax $.IsEqual'));

DECLARE @IsNotEqual BIT = TRY_CONVERT(BIT, JSON_VALUE(@Body,'lax $.IsNotEqual'));

DECLARE @IsNegation BIT = ISNULL(TRY_CONVERT(BIT, JSON_VALUE(@Body,'lax $.IsNegation')),0);

DECLARE @IsDateFieldParameter BIT = ISNULL(TRY_CONVERT(BIT, JSON_VALUE(@Body,'lax $.IsDateFieldParameter')),0);

IF (@IsNullOperator = 1 AND (@IsNegation = 0 AND @IsEqual = 1) OR (@IsNegation = 1 AND @IsEqual = 0))
BEGIN
   WITH EntitySet(Id) AS(
   SELECT Id FROM [dbo].[Account]
   EXCEPT
   SELECT EntityId FROM [dbo].[AccountProperty] WHERE [KeyId] = @PropertyKey
   )
   INSERT @Results (Id, Position)
   SELECT Id, @OutputPosition FROM EntitySet
   RETURN;
END

IF (@IsNullOperator = 1 AND (@IsNegation = 0 AND @IsEqual = 0) OR (@IsNegation = 1 AND @IsEqual = 1))
BEGIN
   WITH EntitySet(Id) AS(
   SELECT DISTINCT EntityId FROM [dbo].[AccountProperty] WHERE [KeyId] = @PropertyKey
   )
   INSERT @Results (Id, Position)
   SELECT Id, @OutputPosition FROM EntitySet;
   RETURN;
END

IF (@IsNegation = 1)
	SET @Operator = CASE @Operator 
		WHEN 'eq' THEN 'ne' 
		WHEN 'ne' THEN 'eq' 
		WHEN 'lt' THEN 'ge' 
		WHEN 'le' THEN 'gt' 
		WHEN 'gt' THEN 'le' 
		WHEN 'ge' THEN 'lt' 
		WHEN 'like' THEN 'nlike' 
		WHEN 'nlike' THEN 'like' 
	END;

IF (@Operator = 'eq')
BEGIN
   WITH EntitySet(Id) AS(
   SELECT DISTINCT EntityId FROM [dbo].[AccountProperty] WHERE [KeyId] = @PropertyKey AND [Value]=@Value
   )
   INSERT @Results (Id, Position)
   SELECT Id, @OutputPosition FROM EntitySet;
   RETURN;
END

IF (@Operator = 'ne')
BEGIN
   WITH EntitySet(Id) AS(
   SELECT DISTINCT EntityId FROM [dbo].[AccountProperty] WHERE [KeyId] = @PropertyKey AND [Value]!=@Value
   )
   INSERT @Results (Id, Position)
   SELECT Id, @OutputPosition FROM EntitySet;
   RETURN;
END

IF (@Operator = 'lt')
BEGIN
   WITH EntitySet(Id) AS(
   SELECT DISTINCT EntityId FROM [dbo].[AccountProperty] WHERE [KeyId] = @PropertyKey AND [Value]<@Value
   )
   INSERT @Results (Id, Position)
   SELECT Id, @OutputPosition FROM EntitySet;
   RETURN;
END

IF (@Operator = 'le')
BEGIN
   WITH EntitySet(Id) AS(
   SELECT DISTINCT EntityId FROM [dbo].[AccountProperty] WHERE [KeyId] = @PropertyKey AND [Value]<=@Value
   )
   INSERT @Results (Id, Position)
   SELECT Id, @OutputPosition FROM EntitySet;
   RETURN;
END

IF (@Operator = 'gt')
BEGIN
   WITH EntitySet(Id) AS(
   SELECT DISTINCT EntityId FROM [dbo].[AccountProperty] WHERE [KeyId] = @PropertyKey AND [Value]>@Value
   )
   INSERT @Results (Id, Position)
   SELECT Id, @OutputPosition FROM EntitySet;
   RETURN;
END

IF (@Operator = 'ge')
BEGIN
   WITH EntitySet(Id) AS(
   SELECT DISTINCT EntityId FROM [dbo].[AccountProperty] WHERE [KeyId] = @PropertyKey AND [Value]>=@Value
   )
   INSERT @Results (Id, Position)
   SELECT Id, @OutputPosition FROM EntitySet;
   RETURN;
END

IF (@Operator = 'like')
BEGIN
   WITH EntitySet(Id) AS(
   SELECT DISTINCT EntityId FROM [dbo].[AccountProperty] WHERE [KeyId] = @PropertyKey AND [Value] LIKE @Value
   )
   INSERT @Results (Id, Position)
   SELECT Id, @OutputPosition FROM EntitySet;
   RETURN;
END

IF (@Operator = 'nlike')
BEGIN
   WITH EntitySet(Id) AS(
   SELECT Id FROM [dbo].[Account]
   EXCEPT
   SELECT EntityId FROM [dbo].[AccountProperty] WHERE [KeyId] = @PropertyKey AND [Value] LIKE @Value
   )
   INSERT @Results (Id, Position)
   SELECT Id, @OutputPosition FROM EntitySet;
   RETURN;
END

RETURN;
END;  
GO
CREATE FUNCTION [dbo].[udfAccountPaginateProperty] (@FullScan BIT, @CollectionId BIGINT, @Body AS NVARCHAR(MAX))  
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
		FROM [dbo].[Account] AS E
		LEFT JOIN [dbo].[AccountSearchHistoryCache] AS C ON E.Id = C.EntityId AND C.[SearchId] = @CollectionId
		LEFT JOIN [dbo].[AccountProperty] AS P ON @IsDateField = 0 AND P.EntityId = C.[EntityId]
		LEFT JOIN [dbo].[AccountPropertyKey] AS PK ON PK.[Type]=@OrderParameter AND P.[KeyId] = PK.[Id]
		WHERE @FullScan=1 OR C.[Id] IS NOT NULL
	)
	INSERT INTO @Results
	SELECT Id, [Rank] FROM Result
	ORDER BY [Rank]
	OFFSET @Skip ROWS
	FETCH NEXT @Top ROWS ONLY
	
	RETURN;
END
GO
