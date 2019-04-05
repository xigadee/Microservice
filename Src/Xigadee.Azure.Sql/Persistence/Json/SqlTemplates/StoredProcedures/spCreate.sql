CREATE PROCEDURE [{NamespaceExternal}].[spCreate{EntityName}]
	@ExternalId UNIQUEIDENTIFIER,
	@VersionId UNIQUEIDENTIFIER = NULL,
	@Body NVARCHAR (MAX),
	@References [{NamespaceExternal}].[KvpTableType] READONLY,
	@Properties [{NamespaceExternal}].[KvpTableType] READONLY
AS
	BEGIN TRY
		BEGIN TRAN



		COMMIT TRAN;
		RETURN 201;
	END TRY
	BEGIN CATCH
		SELECT  ERROR_NUMBER() AS ErrorNumber, ERROR_MESSAGE() AS ErrorMessage; 
		ROLLBACK TRAN;
		RETURN 500;
	END CATCH
GO