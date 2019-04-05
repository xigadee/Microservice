CREATE PROCEDURE [{NamespaceExternal}].[spReadByRef{EntityName}]
	@RefType VARCHAR(30),
	@RefValue NVARCHAR(250) 
AS
SET NOCOUNT ON;
	BEGIN TRY
	
		SELECT E.* 
		FROM [{NamespaceEntity}].[{EntityName}] E
		INNER JOIN [{NamespaceEntity}].[{EntityName}Reference] R ON E.Id = R.ReferenceKeyId
		INNER JOIN [{NamespaceEntity}].[{EntityName}ReferenceKey] RK ON R.ReferenceKeyId = CRK.Id
		WHERE RK.[Type] = @RefType AND R.[Value] = @RefValue

		IF (@@ROWCOUNT>0)
			RETURN 200;
	
		--Not found.
		RETURN 404;

	END TRY
	BEGIN CATCH
		SELECT  ERROR_NUMBER() AS ErrorNumber, ERROR_MESSAGE() AS ErrorMessage;
		RETURN 500
	END CATCH