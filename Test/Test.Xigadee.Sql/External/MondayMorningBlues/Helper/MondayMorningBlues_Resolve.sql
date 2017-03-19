CREATE PROCEDURE [dbo].[MondayMorningBlues_Resolve]
	@ExternalId UNIQUEIDENTIFIER,
	@Id BIGINT OUTPUT,
	@VersionId UNIQUEIDENTIFIER OUTPUT
AS
BEGIN
	SELECT @Id=Id, @VersionId=VersionId FROM [dbo].[MondayMorningBlues] 
	WHERE [ExternalId]=@ExternalId

	IF (@@ROWCOUNT>0)
		RETURN 200;
	--Not found.
	RETURN 404;

END
