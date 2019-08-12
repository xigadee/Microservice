CREATE PROCEDURE [{NamespaceExternal}].[{spSearch}Entity_Default_Json]
	@Body NVARCHAR(MAX)
AS
BEGIN
	BEGIN TRY

		DECLARE @ETag UNIQUEIDENTIFIER = NEWID();
		DECLARE @Result INT = 405;


		EXEC [{NamespaceTable}].spSearchLog @ETag, '{EntityName}', '{spSearch}Entity_Json', @Body;


		RETURN @Result;
	END TRY
	BEGIN CATCH
		SELECT ERROR_NUMBER() AS ErrorNumber, ERROR_MESSAGE() AS ErrorMessage; 
		RETURN 500;
	END CATCH
END