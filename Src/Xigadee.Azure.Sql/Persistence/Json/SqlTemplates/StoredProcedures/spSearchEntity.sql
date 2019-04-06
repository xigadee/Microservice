CREATE PROCEDURE [{NamespaceExternal}].[spCreate{EntityName}]
	 @ExternalId UNIQUEIDENTIFIER
	,@VersionId UNIQUEIDENTIFIER = NULL
	,@VersionIdNew UNIQUEIDENTIFIER = NULL
	,@Body NVARCHAR (MAX)
	,@DateCreated DATETIME = NULL
	,@DateUpdated DATETIME = NULL
	,@References [{NamespaceExternal}].[KvpTableType] READONLY
	,@Properties [{NamespaceExternal}].[KvpTableType] READONLY
	,@Sig VARCHAR(256) = NULL
AS
	BEGIN TRY
		BEGIN TRAN

		-- Insert record into DB and get its identity
		INSERT INTO [{NamespaceTable}].[{EntityName}] 
		(
			  ExternalId
			, VersionId
			, DateCreated
			, DateUpdated
			, Sig
			, Body
		)
		VALUES 
		(
			  @ExternalId
			, ISNULL(@VersionIdNew, NEWID())
			, ISNULL(@DateCreated, GETUTCDATE())
			, @DateUpdated
			, @Sig
			, @Body
		)
		DECLARE @Id BIGINT = SCOPE_IDENTITY();
	
		-- Create references and properties
		DECLARE @RPResponse INT;
		EXEC @RPResponse = [{NamespaceTable}].[spUpsert{EntityName}PropertyReferences] @Id, @References, @Properties
		IF (@RPResponse = 409)
		BEGIN
			ROLLBACK TRAN;
			RETURN 409; --Conflict with other entities.
		END

		COMMIT TRAN;

		RETURN 201;
	END TRY
	BEGIN CATCH
		SELECT  ERROR_NUMBER() AS ErrorNumber, ERROR_MESSAGE() AS ErrorMessage; 
		ROLLBACK TRAN;
		RETURN 500;
	END CATCH
