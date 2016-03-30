CREATE PROCEDURE [External].[MondayMorningBluesVersionByReference]
	@Email NVARCHAR(150)
AS
BEGIN
	DECLARE @Id BIGINT,@ResolveStatus INT, @VersionId UNIQUEIDENTIFIER, @ExternalId UNIQUEIDENTIFIER

	EXEC @ResolveStatus = [dbo].[MondayMorningBlues_ResolveByRef] @Email
		, @Id OUTPUT
		, @ExternalId OUTPUT
		, @VersionId OUTPUT
		
	IF (@ResolveStatus != 200)
		RETURN @ResolveStatus;

	SELECT [dbo].[FnEntityVersion] (@ExternalId, @VersionId)
	 
	RETURN 200;
END