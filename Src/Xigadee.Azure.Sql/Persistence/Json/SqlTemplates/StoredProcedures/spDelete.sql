CREATE PROCEDURE [{NamespaceExternal}].[{spDelete}]
	@ExternalId UNIQUEIDENTIFIER
AS
SET NOCOUNT ON;
	BEGIN TRY
	
		BEGIN TRAN

		DECLARE @Id BIGINT = (SELECT [Id] FROM [{NamespaceTable}].[{EntityName}] WHERE [ExternalId] = @ExternalId)

		IF (@Id IS NULL)
		BEGIN
			ROLLBACK TRAN
			RETURN 404;
		END

		--#region.extension
		DELETE FROM [{NamespaceTable}].[{EntityName}_Extension]
		WHERE [EntityId] = @Id
		--#endregion

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
		ROLLBACK TRAN;
		RETURN 500;
	END CATCH