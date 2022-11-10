﻿CREATE PROCEDURE [{NamespaceExternal}].[{spReadByRef}]
	@RefType VARCHAR(30),
	@RefValue NVARCHAR(250) 
AS
SET NOCOUNT ON;
	BEGIN TRY
	
		SELECT E.[Sig],E.[Body] 
		FROM [{NamespaceTable}].[{EntityName}] E
		INNER JOIN [{NamespaceTable}].[{EntityName}Reference] R ON E.Id = R.EntityId
		INNER JOIN [{NamespaceTable}].[{EntityName}ReferenceKey] RK ON R.KeyId = RK.Id
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