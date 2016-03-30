CREATE PROCEDURE [External].[MondayMorningBluesRead]
	@ExternalId UNIQUEIDENTIFIER
AS
BEGIN
	DECLARE @Id BIGINT,@ResolveStatus INT, @VersionId UNIQUEIDENTIFIER

	EXEC @ResolveStatus = [dbo].[MondayMorningBlues_Resolve] @ExternalId, @Id OUTPUT, @VersionId OUTPUT
	if (@ResolveStatus != 200)
		RETURN @ResolveStatus;

	SELECT [dbo].[FnMondayMorningBlues] (@Id)
	 
	RETURN 200;
END
