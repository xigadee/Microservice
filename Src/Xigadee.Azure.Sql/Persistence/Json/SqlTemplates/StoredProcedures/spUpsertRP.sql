CREATE PROCEDURE [{NamespaceTable}].[spUpsert{EntityName}PropertyReferences]
	@EntityId BIGINT,
	@References [{NamespaceExternal}].[KvpTableType] READONLY,
	@Properties [{NamespaceExternal}].[KvpTableType] READONLY
AS
	BEGIN TRY

		--Process the reference first.
		INSERT INTO [{NamespaceTable}].[{EntityName}ReferenceKey] WITH (UPDLOCK) ([Type])
		SELECT DISTINCT [RefType] FROM @References
		EXCEPT
		SELECT [Type] FROM [{NamespaceTable}].[{EntityName}ReferenceKey]

		--Remove old records
		DELETE FROM [{NamespaceTable}].[{EntityName}Reference] WITH (UPDLOCK)
		WHERE [EntityId] = @EntityId

		--Add the new records.
		INSERT INTO [{NamespaceTable}].[{EntityName}Reference] WITH (UPDLOCK)
		(
		      [EntityId]
			, [KeyId]
			, [Value]
		)
		SELECT @EntityId, K.[Id], R.RefValue
		FROM @References AS R 
		INNER JOIN [{NamespaceTable}].[{EntityName}ReferenceKey] AS K ON R.[RefType] = K.[Type]
	
		--Now process the properties.
		INSERT INTO [{NamespaceTable}].[{EntityName}PropertyKey] WITH (UPDLOCK) ([Type])
		SELECT DISTINCT [RefType] FROM @Properties
		EXCEPT
		SELECT [Type] FROM [{NamespaceTable}].[{EntityName}PropertyKey]

		--Remove old records
		DELETE FROM [{NamespaceTable}].[{EntityName}Property] WITH (UPDLOCK)
		WHERE [EntityId] = @EntityId

		--Add the new records.
		INSERT INTO [{NamespaceTable}].[{EntityName}Property] WITH (UPDLOCK)
		(
			  [EntityId]
			, [KeyId]
			, [Value]
		)
		SELECT @EntityId, K.[Id], P.RefValue
		FROM @Properties AS P
		INNER JOIN [{NamespaceTable}].[{EntityName}PropertyKey] AS K ON P.[RefType] = K.[Type]

		RETURN 200;
	END TRY
	BEGIN CATCH
		IF (ERROR_NUMBER() = 2601)
			RETURN 409; --Conflict - duplicate reference key to other transaction.
		ELSE
			THROW;
	END CATCH
