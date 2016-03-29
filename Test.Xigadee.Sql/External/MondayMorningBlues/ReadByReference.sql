CREATE PROCEDURE [External].[MondayMorningBluesReadByReference]
	@Email NVARCHAR(150)
AS
BEGIN
	DECLARE @Id BIGINT,@ResolveStatus INT

	EXEC @ResolveStatus = [dbo].[MondayMorningBlues_ResolveByRef] @Email, @Id OUTPUT
	if (@ResolveStatus != 200)
		RETURN @ResolveStatus;

	SELECT [dbo].[FnMondayMorningBlues] (@Id)
	 
	RETURN 200;
END
