CREATE PROCEDURE [Datapump].[Resolve_Instance]
	 @Name NVARCHAR(250)
	,@Id INT OUTPUT
AS
BEGIN

	SELECT @Id = [Id] FROM [Datapump].[Instance] WHERE [Name]=@Name

	IF (@Id IS NULL)
	BEGIN
		INSERT INTO [Datapump].[Instance] ([Name]) VALUES (@Name)

		SELECT @Id = CAST(SCOPE_IDENTITY() AS INT);
	END
END
