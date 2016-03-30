CREATE PROCEDURE [External].[MondayMorningBluesDelete]
	 @ExternalId UNIQUEIDENTIFIER
AS
BEGIN

	DECLARE @Id BIGINT,@ResolveStatus INT, @VersionId UNIQUEIDENTIFIER

	EXEC @ResolveStatus = [dbo].[MondayMorningBlues_Resolve] @ExternalId, @Id OUTPUT, @VersionId OUTPUT
	if (@ResolveStatus != 200)
		RETURN @ResolveStatus;
	
	BEGIN TRY;
		BEGIN TRAN;

		DELETE FROM [dbo].[MondayMorningBlues]
		WHERE [Id] = @Id

		COMMIT TRAN
		IF (@@ROWCOUNT > 0)
			RETURN 200;
		ELSE 
			RETURN 404;	
	END TRY
	BEGIN CATCH	
		 ROLLBACK TRAN
		 RETURN 500;
	END CATCH	

END
