CREATE PROCEDURE [{NamespaceTable}].spSearchLog
	 @ETag UNIQUEIDENTIFIER
	,@EntityType VARCHAR(50)
	,@SearchType VARCHAR(50)
	,@Body NVARCHAR(MAX)
AS
BEGIN

INSERT INTO [{NamespaceTable}].[SearchHistory]
([ETag],[EntityType],[SearchType],[Sig],[Body])
VALUES
(@ETag, @EntityType, @SearchType, '', @Body);


END