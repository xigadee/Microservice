﻿CREATE PROCEDURE [{NamespaceExternal}].[{spSearch}_Default]
	@ETag VARCHAR(50),
	@PropertiesFilter [{NamespaceExternal}].[KvpTableType] READONLY,
	@PropertyOrder [{NamespaceExternal}].[KvpTableType] READONLY,
	@Skip INT = 0,
	@Top INT = 50
AS
BEGIN
	BEGIN TRY
		--Build
		DECLARE @FilterIds TABLE
		(
			Id BIGINT,
			Score INT
		);

		INSERT INTO @FilterIds
			EXEC [{NamespaceTable}].[{spSearch}InternalBuild_Default] @PropertiesFilter, @PropertyOrder, @Skip, @Top

		SELECT * FROM @FilterIds;

		RETURN 200;
	END TRY
	BEGIN CATCH
		SELECT  ERROR_NUMBER() AS ErrorNumber, ERROR_MESSAGE() AS ErrorMessage; 
		ROLLBACK TRAN;
		RETURN 500;
	END CATCH
END