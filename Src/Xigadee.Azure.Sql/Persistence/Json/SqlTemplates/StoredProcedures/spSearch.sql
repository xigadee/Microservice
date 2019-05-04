CREATE PROCEDURE [{NamespaceExternal}].[{spSearch}_Default]
	@PropertiesFilter [{NamespaceExternal}].[KvpTableType] READONLY,
	@PropertyOrder [{NamespaceExternal}].[KvpTableType] READONLY,
	@Skip INT = 0,
	@Take INT = 50
AS
	BEGIN TRY

	--Build
	DECLARE @FilterIds TABLE
	(
		Id BIGINT,
		Score INT

	);

	--Build
	INSERT INTO @FilterIds
	SELECT E.Id
	FROM [{NamespaceTable}].[{EntityName}] E
	INNER JOIN [{NamespaceTable}].[{EntityName}Property] P ON E.Id = P.EntityId
	INNER JOIN [{NamespaceTable}].[{EntityName}PropertyKey] PK ON P.KeyId = PK.Id
	WHERE PK.[Type] = @RefType AND R.[Value] = @RefValue

		--Output
		INSERT INTO @FilterIds
		SELECT E.Id
		FROM [{NamespaceTable}].[{EntityName}] E
		INNER JOIN [{NamespaceTable}].[{EntityName}Property] P ON E.Id = P.EntityId
		INNER JOIN [{NamespaceTable}].[{EntityName}PropertyKey] PK ON P.KeyId = PK.Id
		WHERE PK.[Type] = @RefType AND R.[Value] = @RefValue
		ORDER BY TCB.TransactionId
		OFFSET @Skip ROWS
		FETCH NEXT @Take ROWS ONLY


		RETURN 200;
	END TRY
	BEGIN CATCH
		SELECT  ERROR_NUMBER() AS ErrorNumber, ERROR_MESSAGE() AS ErrorMessage; 
		ROLLBACK TRAN;
		RETURN 500;
	END CATCH
