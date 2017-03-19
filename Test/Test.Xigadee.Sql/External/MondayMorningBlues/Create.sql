CREATE PROCEDURE [External].[MondayMorningBluesCreate]
	 @ExternalId UNIQUEIDENTIFIER	
	,@Data XML
AS
BEGIN
	DECLARE @Id BIGINT,@ResolveStatus INT, @VersionId UNIQUEIDENTIFIER

	EXEC @ResolveStatus = [dbo].[MondayMorningBlues_Resolve] @ExternalId, @Id OUTPUT, @VersionId OUTPUT
	if (@ResolveStatus = 200)
		RETURN 412;

	BEGIN TRY
		BEGIN TRAN;	

		INSERT INTO [dbo].[MondayMorningBlues]
			   ([ExternalId]
			   ,[VersionId]
			   ,[NotEnoughCoffee]
			   ,[NotEnoughSleep]
			   ,[Message]
			   ,[Email]
			   ,[ContentId]
			   ,[DateCreated]
			   ,[DateUpdated]
			   )
		 SELECT
			    T.N.value('@ExternalId', 'UNIQUEIDENTIFIER')
			   ,T.N.value('@VersionId', 'UNIQUEIDENTIFIER')
			   ,T.N.value('@NotEnoughCoffee', 'BIT')
			   ,T.N.value('@NotEnoughSleep', 'BIT')
			   ,T.N.value('@Message', 'nvarchar(250)')
			   ,T.N.value('@Email', 'nvarchar(150)')
			   ,T.N.value('@ContentId', 'UNIQUEIDENTIFIER')
			   ,ISNULL(T.N.value('@DateCreated', 'DATETIME'), GETUTCDATE())
			   ,ISNULL(T.N.value('@DateUpdated', 'DATETIME'), null)

			FROM @Data.nodes('/MondayMorningBlues') as T(N)		
				
			SELECT @Id = SCOPE_IDENTITY();
				
			COMMIT TRAN;
	END TRY
	BEGIN CATCH
		 --DECLARE @ErrorXml XML = (SELECT [Core].[fnFormatError]())
		 ROLLBACK TRAN
		 --EXEC [dbo].[DatabaseLog_Create] @ErrorXml
		 --EXEC [Core].[Security] @EntityType='CUSTOMEROFFER',@Action='CREATE:FAILERROR', @Data=@Data, @Status=500, @ExternalId=@ExternalId
		 RETURN 500;
	END CATCH
	
	SELECT [dbo].[fnMondayMorningBlues] (@Id)

	RETURN 201;
END