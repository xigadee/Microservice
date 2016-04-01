CREATE PROCEDURE [External].[MondayMorningBluesDelete]
	 @ExternalId UNIQUEIDENTIFIER
AS
BEGIN

	DECLARE @Id BIGINT, @ResolveStatus INT, @VersionId UNIQUEIDENTIFIER

	EXEC @ResolveStatus = [dbo].[MondayMorningBlues_Resolve] @ExternalId, @Id OUTPUT, @VersionId OUTPUT
	if (@ResolveStatus != 200)
		RETURN @ResolveStatus;
	
	BEGIN TRY;
		BEGIN TRAN;

		DELETE FROM [dbo].[MondayMorningBlues]
		WHERE [Id] = @Id

		COMMIT TRAN
				
		IF (@@ROWCOUNT = 0)
			RETURN 404;
	END TRY
	BEGIN CATCH	
		 ROLLBACK TRAN
		 RETURN 500;
	END CATCH	
	
	SELECT [dbo].[fnEntityVersion] (@ExternalId, @VersionId)
	 
	RETURN 200;
END
