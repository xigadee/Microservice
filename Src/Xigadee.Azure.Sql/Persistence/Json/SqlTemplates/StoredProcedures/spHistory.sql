CREATE PROCEDURE [{NamespaceTable}].[{spHistory}]
	 @EntityId BIGINT
	,@ExternalId UNIQUEIDENTIFIER
	,@VersionId UNIQUEIDENTIFIER 
	,@VersionIdNew UNIQUEIDENTIFIER 
    ,@UserIdAudit UNIQUEIDENTIFIER 
	,@Body NVARCHAR (MAX)
	,@DateCreated DATETIME 
	,@DateUpdated DATETIME 
	,@Sig VARCHAR(256) 
AS
	BEGIN TRY

		--Process the reference first.
		INSERT INTO [{NamespaceTable}].[{EntityName}History] 
		(
			 [EntityId]
			,[ExternalId] 
			,[VersionId] 
			,[UserIdAudit] 
			,[DateCreated] 
			,[DateUpdated] 
			,[Sig] 
			,[Body]
		)
		VALUES
		(
			 @EntityId 
			,@ExternalId 
			,@VersionIdNew  
			,@UserIdAudit  
			,@DateCreated  
			,@DateUpdated  
			,@Sig 
			,@Body 
		)

		RETURN 200;
	END TRY
	BEGIN CATCH
		IF (ERROR_NUMBER() = 2601)
			RETURN 409; --Conflict - duplicate reference key to other transaction.
		ELSE
			THROW;
	END CATCH
