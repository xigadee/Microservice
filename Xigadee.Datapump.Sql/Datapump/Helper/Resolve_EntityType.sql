CREATE PROCEDURE [Datapump].[Resolve_EntityType]
	 @Name VARCHAR(250)
	,@Id SMALLINT OUTPUT
AS
BEGIN

	SELECT @Id = [Id] FROM [Datapump].[EntityType] WHERE [Name]=@Name

	IF (@Id IS NULL)
	BEGIN
		INSERT INTO [Datapump].[EntityType] ([Name]) VALUES (@Name)

		SELECT @Id = CAST(SCOPE_IDENTITY() AS SMALLINT);
	END
END
