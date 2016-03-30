CREATE PROCEDURE [External].[MondayMorningBluesDeleteByReference]
	 @Email NVARCHAR(150)
AS
BEGIN

	DECLARE @Id BIGINT,@ResolveStatus INT, @VersionId UNIQUEIDENTIFIER, @ExternalId UNIQUEIDENTIFIER

	EXEC @ResolveStatus = [dbo].[MondayMorningBlues_ResolveByRef] @Email
		, @Id OUTPUT
		, @ExternalId OUTPUT
		, @VersionId OUTPUT

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
