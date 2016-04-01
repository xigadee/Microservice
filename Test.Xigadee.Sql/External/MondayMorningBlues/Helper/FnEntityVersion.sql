CREATE FUNCTION [dbo].[fnEntityVersion]
(
	@ExternalId UNIQUEIDENTIFIER,
	@VersionId UNIQUEIDENTIFIER
)
RETURNS XML
AS
BEGIN
	DECLARE @Output XML =
	(
		SELECT	@VersionId as '@VersionId',
				@ExternalId as '@ExternalId'				
		FOR XML PATH ('Entity'), TYPE
	)

	RETURN @Output
END
