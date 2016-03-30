CREATE PROCEDURE [External].[MondayMorningBluesUpdate]
	 @ExternalId UNIQUEIDENTIFIER	
	,@Data XML
AS
BEGIN
	DECLARE @Id BIGINT,@ResolveStatus INT

	EXEC @ResolveStatus = [dbo].[MondayMorningBlues_Resolve] @ExternalId, @Id OUTPUT
	if (@ResolveStatus != 200)
		RETURN 404;

	BEGIN TRY

		BEGIN TRAN;

		;WITH XmlFlat(
			 [ExternalId]
			,[VersionId]
			,[NotEnoughCoffee]
			,[NotEnoughSleep]
			,[Message]
			,[Email]
			,[ContentId]
			,[DateUpdated]
		)AS
		(
			SELECT 
				@ExternalId
			   ,T.N.value('@VersionId', 'UNIQUEIDENTIFIER')
			   ,T.N.value('@NotEnoughCoffee', 'BIT')
			   ,T.N.value('@NotEnoughSleep', 'BIT')
			   ,T.N.value('@Message', 'nvarchar(250)')
			   ,T.N.value('@Email', 'nvarchar(150)')
			   ,T.N.value('@ContentId', 'UNIQUEIDENTIFIER')
			   ,ISNULL(T.N.value('@DateUpdated', 'DATETIME'), GETUTCDATE())
			FROM @Data.nodes('/MondayMorningBlues') as T(N)		
		)	
		UPDATE c
		SET
			 [VersionId] = NEWID()
		    ,[DateUpdated] = GETUTCDATE()

						
		FROM [dbo].[MondayMorningBlues] AS c
		INNER JOIN XmlFlat AS cf ON cf.ExternalId=c.ExternalId			
		WHERE c.Id=@Id 
		AND c.VersionId = cf.VersionId

		IF (@@ROWCOUNT < 1) 
		BEGIN
			ROLLBACK TRAN;
			RETURN 409; -- Cannot update, conflict with current version.
		END
		COMMIT TRAN;

	END TRY
	BEGIN CATCH
		 ROLLBACK TRAN
		 RETURN 500;
	END CATCH

	SELECT [dbo].[FnMondayMorningBlues] (@Id)

	RETURN 200;
END
