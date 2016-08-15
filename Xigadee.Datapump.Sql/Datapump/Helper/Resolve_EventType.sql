CREATE PROCEDURE [Datapump].[Resolve_EventType]
	 @Name VARCHAR(50)
	,@Id SMALLINT OUTPUT
AS
BEGIN

	SELECT @Id = [Id] FROM [Datapump].[EventType] WHERE [Name]=@Name

	IF (@Id IS NULL)
	BEGIN
		INSERT INTO [Datapump].[EventType] ([Name]) VALUES (@Name)

		SELECT @Id = CAST(SCOPE_IDENTITY() AS SMALLINT);
	END
END
