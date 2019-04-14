CREATE PROCEDURE [{NamespaceExternal}].[{spUpdate}]
	 @ExternalId UNIQUEIDENTIFIER
	,@VersionId UNIQUEIDENTIFIER = NULL
	,@VersionIdNew UNIQUEIDENTIFIER = NULL
    ,@UserIdAudit UNIQUEIDENTIFIER = NULL 
	,@Body NVARCHAR (MAX)
	,@DateCreated DATETIME = NULL
	,@DateUpdated DATETIME = NULL
	,@References [{NamespaceExternal}].[KvpTableType] READONLY
	,@Properties [{NamespaceExternal}].[KvpTableType] READONLY
	,@Sig VARCHAR(256) = NULL
AS
	BEGIN TRY
		BEGIN TRAN

		DECLARE @Id BIGINT

		IF (@VersionId IS NOT NULL)
		BEGIN
			DECLARE @ExistingVersion UNIQUEIDENTIFIER;
			SELECT @Id = [Id], @VersionId = [VersionId] FROM [{NamespaceTable}].[{EntityName}] WHERE [ExternalId] = @ExternalId
			IF (@Id IS NOT NULL AND @ExistingVersion != @VersionId)
			BEGIN
				ROLLBACK TRAN;
				RETURN 409; --Conflict
			END
		END
		ELSE
		BEGIN
			SELECT @Id = [Id] FROM [{NamespaceTable}].[{EntityName}] WHERE [ExternalId] = @ExternalId
		END

		--Check we can find the entity
		IF (@Id IS NULL)
		BEGIN
			ROLLBACK TRAN;
			RETURN 404; --Not found
		END

		-- Insert record into DB and get its identity
		UPDATE [{NamespaceTable}].[{EntityName}]
		SET   [VersionId] = @VersionIdNew
			, [UserIdAudit] = @UserIdAudit
			, [DateUpdated] = ISNULL(@DateUpdated, GETUTCDATE())
			, [Sig] = @Sig
			, [Body] = @Body
		WHERE Id = @Id

		IF (@@ROWCOUNT=0)
		BEGIN
			ROLLBACK TRAN;
			RETURN 409; -- Update failure
		END
	
		-- Create references and properties
		DECLARE @RPResponse INT;
		EXEC @RPResponse = [{NamespaceTable}].[{spUpsertRP}] @Id, @References, @Properties
		IF (@RPResponse = 409)
		BEGIN
			ROLLBACK TRAN;
			RETURN 409; --Not found
		END

	    --Record the audit history.
		EXEC [{NamespaceTable}].[{spHistory}] @Id, @ExternalId, @VersionIdNew, @UserIdAudit, @Body, @DateCreated, @DateUpdated, @Sig

		COMMIT TRAN;

		RETURN 200;
	END TRY
	BEGIN CATCH
		SELECT  ERROR_NUMBER() AS ErrorNumber, ERROR_MESSAGE() AS ErrorMessage; 
		ROLLBACK TRAN;

		RETURN 500;
	END CATCH
