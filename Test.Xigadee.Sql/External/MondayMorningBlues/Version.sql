CREATE PROCEDURE [External].[MondayMorningBluesVersion]
	@ExternalId UNIQUEIDENTIFIER
AS
BEGIN
	DECLARE @Id BIGINT,@ResolveStatus INT, @VersionId UNIQUEIDENTIFIER

	EXEC @ResolveStatus = [dbo].[MondayMorningBlues_Resolve] @ExternalId
		, @Id OUTPUT
		, @VersionId OUTPUT

	if (@ResolveStatus != 200)
		RETURN @ResolveStatus;

	SELECT [dbo].[FnEntityVersion] (@ExternalId, @VersionId)
	 
	RETURN 200;
END