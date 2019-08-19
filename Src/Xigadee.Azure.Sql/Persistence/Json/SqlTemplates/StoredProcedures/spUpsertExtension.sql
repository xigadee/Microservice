--#region.extension
CREATE PROCEDURE [{NamespaceTable}].[{spUpsertRP}_Extension]
	 @EntityId BIGINT
	,@Update BIT
	,@Body NVARCHAR (MAX)
AS
BEGIN
		IF (@Update = 0)
		BEGIN
			-- Insert record into DB and get its identity
			INSERT INTO [{NamespaceTable}].[{EntityName}_Extension] 
			(
				  EntityId
			)
			VALUES 
			(
				  @EntityId
			)

			RETURN 201;
		END
		ELSE
		BEGIN
			--UPDATE [{NamespaceTable}].[{EntityName}_Extension] 
			--	SET Something='';
			--WHERE EntityId = @EntityId
			RETURN 200;
		END
	
END
GO
--#endregion