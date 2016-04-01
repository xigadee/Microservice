CREATE PROCEDURE [External].[MondayMorningBluesDeleteByRef]
	  @RefType NVARCHAR(50)
	, @RefValue NVARCHAR(255)
AS
BEGIN

	DECLARE @Id BIGINT,@ResolveStatus INT, @VersionId UNIQUEIDENTIFIER, @ExternalId UNIQUEIDENTIFIER
	
	IF (@RefType != 'EMAIL')
		RETURN 404;

	EXEC @ResolveStatus = [dbo].[MondayMorningBlues_ResolveByRef] @RefValue
		, @Id OUTPUT
		, @ExternalId OUTPUT
		, @VersionId OUTPUT

	if (@ResolveStatus != 200)
		RETURN @ResolveStatus;
	
	BEGIN TRY;

		DELETE FROM [dbo].[MondayMorningBlues]
		WHERE [Id] = @Id

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
