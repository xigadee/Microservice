CREATE PROCEDURE [{NamespaceExternal}].[{spSearch}Entity_Json]
	@Body NVARCHAR(MAX)
AS
BEGIN
	BEGIN TRY

		DECLARE @ETag UNIQUEIDENTIFIER = NEWID();
		DECLARE @Result INT = 405;


		EXEC [{NamespaceInternal}].spSearchLog @ETag, '{EntityName}', '{spSearch}Entity_Json',@Result, @Body;


		RETURN @Result;
	END TRY
	BEGIN CATCH
		SELECT ERROR_NUMBER() AS ErrorNumber, ERROR_MESSAGE() AS ErrorMessage; 
		RETURN 500;
	END CATCH
END