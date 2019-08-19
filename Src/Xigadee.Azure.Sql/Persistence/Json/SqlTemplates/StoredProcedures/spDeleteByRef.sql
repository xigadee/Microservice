CREATE PROCEDURE [{NamespaceExternal}].[{spDeleteByRef}]
	@RefType VARCHAR(30),
	@RefValue NVARCHAR(250) 
AS
SET NOCOUNT ON;
	BEGIN TRY
		
		BEGIN TRAN

		DECLARE @Id BIGINT = (
			SELECT E.[Id] FROM [{NamespaceTable}].[{EntityName}] E
			INNER JOIN [{NamespaceTable}].[{EntityName}Reference] R ON E.Id = R.EntityId
			INNER JOIN [{NamespaceTable}].[{EntityName}ReferenceKey] RK ON R.KeyId = RK.Id
			WHERE RK.[Type] = @RefType AND R.[Value] = @RefValue
		)

		IF (@Id IS NULL)
		BEGIN
			ROLLBACK TRAN
			RETURN 404;
		END

		DELETE FROM [{NamespaceTable}].[{EntityName}_Extension]
		WHERE [EntityId] = @Id

		DELETE FROM [{NamespaceTable}].[{EntityName}Property]
		WHERE [EntityId] = @Id

		DELETE FROM [{NamespaceTable}].[{EntityName}Reference]
		WHERE [EntityId] = @Id

		DELETE FROM [{NamespaceTable}].[{EntityName}]
		WHERE [Id] = @Id

		--Not found.
		COMMIT TRAN

		RETURN 200;

	END TRY
	BEGIN CATCH
		SELECT  ERROR_NUMBER() AS ErrorNumber, ERROR_MESSAGE() AS ErrorMessage;
		ROLLBACK TRAN
		RETURN 500
	END CATCH