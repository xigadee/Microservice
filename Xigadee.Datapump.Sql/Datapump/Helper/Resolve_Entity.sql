CREATE PROCEDURE [Datapump].[Resolve_Entity]
	 @EntityType VARCHAR(250)
	,@Key VARCHAR(250)
	,@EntityId BIGINT OUTPUT
	,@EntityTypeId SMALLINT OUTPUT
AS
BEGIN

	EXEC [Datapump].[Resolve_EntityType] @EntityType, @EntityTypeId OUTPUT

	SELECT @EntityId = [Id] FROM [Datapump].[Entity] WHERE [EntityType]=@EntityTypeId AND [Key]=@Key ;

	IF (@EntityId IS NULL)
	BEGIN
		INSERT INTO [Datapump].[Entity] ([EntityType], [Key]) VALUES (@EntityTypeId, @Key)

		SELECT @EntityId = CAST(SCOPE_IDENTITY() AS BIGINT);
	END
END
