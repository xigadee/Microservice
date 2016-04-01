CREATE FUNCTION [dbo].[fnMondayMorningBlues]
(
	@Id BIGINT
)
RETURNS XML
AS
BEGIN

	DECLARE @Output XML

	SELECT @Output =
	(
		SELECT	 c.[ExternalId] as '@ExternalId'
				,c.[NotEnoughCoffee] AS '@NotEnoughCoffee'
				,c.[NotEnoughSleep] AS '@NotEnoughSleep'
				,c.[Message] AS '@Message'
				,c.[Email] AS '@Email'		
				,c.[DateCreated] AS '@DateCreated'						
				,c.[DateUpdated] AS '@DateUpdated'						
				,c.[VersionId] AS '@VersionId'
				,c.[ContentId] AS '@ContentId'
		FROM dbo.[MondayMorningBlues] AS c
		WHERE c.Id = @Id 		
		FOR XML PATH ('MondayMorningBlues'), TYPE
	)
	RETURN @Output
END

