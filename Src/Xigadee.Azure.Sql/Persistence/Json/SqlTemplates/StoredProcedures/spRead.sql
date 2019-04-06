CREATE PROCEDURE [{NamespaceExternal}].[{spRead}]
	@ExternalId UNIQUEIDENTIFIER
AS
SET NOCOUNT ON;
	BEGIN TRY
	
		SELECT * 
		FROM [{NamespaceTable}].[{EntityName}]
		WHERE [ExternalId] = @ExternalId

		IF (@@ROWCOUNT>0)
			RETURN 200;
	
		--Not found.
		RETURN 404;

	END TRY
	BEGIN CATCH
		SELECT  ERROR_NUMBER() AS ErrorNumber, ERROR_MESSAGE() AS ErrorMessage;
		RETURN 500
	END CATCH