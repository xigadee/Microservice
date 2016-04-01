CREATE PROCEDURE [External].[MondayMorningBluesReadByRef]
	  @RefType NVARCHAR(50)
	, @RefValue NVARCHAR(255)
AS
BEGIN
	DECLARE @Id BIGINT,@ResolveStatus INT, @VersionId UNIQUEIDENTIFIER, @ExternalId UNIQUEIDENTIFIER

	IF (@RefType != 'EMAIL')
		RETURN 404;

	EXEC @ResolveStatus = [dbo].[MondayMorningBlues_ResolveByRef] @RefValue, @Id OUTPUT, @ExternalId OUTPUT, @VersionId OUTPUT

	if (@ResolveStatus != 200)
		RETURN @ResolveStatus;

	SELECT [dbo].[fnMondayMorningBlues] (@Id)
	 
	RETURN 200;
END
