CREATE PROCEDURE [External].[MondayMorningBluesDeleteByReference]
	 @Email NVARCHAR(150)
AS
BEGIN

	DECLARE @Id BIGINT,@ResolveStatus INT

	EXEC @ResolveStatus = [dbo].[MondayMorningBlues_ResolveByRef] @Email, @Id OUTPUT
	if (@ResolveStatus != 200)
		RETURN @ResolveStatus;
	
	BEGIN TRY;
		BEGIN TRAN;

		DELETE FROM [dbo].[MondayMorningBlues]
		WHERE [Id] = @Id

		COMMIT TRAN

		RETURN 200	
	END TRY
	BEGIN CATCH	
		 --DECLARE @ErrorXml XML = (SELECT [Core].[fnFormatError]())
		 ROLLBACK TRAN
		 --EXEC [dbo].[DatabaseLog_Create] @ErrorXml
		 RETURN 500;
	END CATCH	
END
