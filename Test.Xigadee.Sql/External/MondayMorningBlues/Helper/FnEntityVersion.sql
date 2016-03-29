CREATE FUNCTION [dbo].[fnEntityVersion]
(
	@VersionId UNIQUEIDENTIFIER,
	@ExternalId UNIQUEIDENTIFIER

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
