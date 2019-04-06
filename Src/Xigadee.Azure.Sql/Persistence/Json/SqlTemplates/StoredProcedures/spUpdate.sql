CREATE PROCEDURE [{NamespaceExternal}].[spUpdate{EntityName}]
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

		IF (@VersionId IS NOT NULL)
		BEGIN
			DECLARE @ExistingVersion UNIQUEIDENTIFIER = (SELECT @VersionId FROM [{NamespaceTable}].[{EntityName}] WHERE [ExternalId] = @ExternalId)
			IF (@ExistingVersion != @VersionId)
			BEGIN
				ROLLBACK TRAN;
				RETURN 409; --Conflict
			END
		END

		-- Insert record into DB and get its identity
		UPDATE [{NamespaceTable}].[{EntityName}]
		SET   [VersionId] = @VersionIdNew
			, [DateUpdated] = ISNULL(@DateUpdated, GETUTCDATE())
			, [Sig] = @Sig
			, [Body] = @Body
		WHERE Id = @ExternalId

		IF (@@ROWCOUNT=0)
		BEGIN
			ROLLBACK TRAN;
			RETURN 409; -- Update failure
		END
	
		-- Create references and properties
		DECLARE @RPResponse INT;
		EXEC @RPResponse = [{NamespaceTable}].[spUpsert{EntityName}PropertyReferences] @Id, @References, @Properties
		IF (@RPResponse = 409)
		BEGIN
			ROLLBACK TRAN;
			RETURN 409; --Not found
		END

		COMMIT TRAN;

		RETURN 201;
	END TRY
	BEGIN CATCH
		SELECT  ERROR_NUMBER() AS ErrorNumber, ERROR_MESSAGE() AS ErrorMessage; 
		ROLLBACK TRAN;

		RETURN 500;
	END CATCH
