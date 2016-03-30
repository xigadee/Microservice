CREATE PROCEDURE [External].[MondayMorningBluesReadByReference]
	@Email NVARCHAR(150)
AS
BEGIN
	DECLARE @Id BIGINT,@ResolveStatus INT, @VersionId UNIQUEIDENTIFIER, @ExternalId UNIQUEIDENTIFIER

	EXEC @ResolveStatus = [dbo].[MondayMorningBlues_ResolveByRef] @Email, @Id OUTPUT
		, @ExternalId OUTPUT, @VersionId OUTPUT

	if (@ResolveStatus != 200)
		RETURN @ResolveStatus;

	SELECT [dbo].[FnMondayMorningBlues] (@Id)
	 
	RETURN 200;
END
