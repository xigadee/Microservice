﻿CREATE PROCEDURE [{NamespaceExternal}].[{spSearch}_Json]
	@Body NVARCHAR(MAX)
AS
BEGIN
	BEGIN TRY
		

		RETURN 405;
	END TRY
	BEGIN CATCH
		SELECT ERROR_NUMBER() AS ErrorNumber, ERROR_MESSAGE() AS ErrorMessage; 
		RETURN 500;
	END CATCH
END