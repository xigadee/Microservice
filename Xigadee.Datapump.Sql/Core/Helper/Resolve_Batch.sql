CREATE PROCEDURE [Core].[Resolve_Batch]
	 @Name UNIQUEIDENTIFIER
	,@Id INT OUTPUT
AS
BEGIN
	
	IF (@Name IS NULL)
		RETURN 200;

	SELECT @Id = [Id] FROM [Core].[Batch] WHERE [BatchId]=@Name

	IF (@Id IS NULL)
	BEGIN
		INSERT INTO [Core].[Batch] ([BatchId]) VALUES (@Name)

		SELECT @Id = CAST(SCOPE_IDENTITY() AS INT);
	END

END

