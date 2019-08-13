CREATE FUNCTION [{NamespaceTable}].[udfFilter{EntityName}Property] (@Body AS NVARCHAR(MAX))  
RETURNS @Results TABLE   
(  
    Id BIGINT PRIMARY KEY NOT NULL,
    Position INT NOT NULL
)  
--Returns a result set that lists all the employees who report to the   
--specific employee directly or indirectly.*/  
AS  
BEGIN  

DECLARE @Parameter VARCHAR(30) = JSON_VALUE(@Body,'lax $.Parameter');
IF (@Parameter IS NULL)
	RETURN;

DECLARE @PropertyKey INT = (SELECT Id FROM [{NamespaceTable}].[{EntityName}PropertyKey] WHERE [Type] = @Parameter);
IF (@PropertyKey IS NULL)
	RETURN;

DECLARE @Operator VARCHAR(30) = JSON_VALUE(@Body,'lax $.Operator');
IF (@Operator IS NULL)
	RETURN;

DECLARE @Position INT = TRY_CONVERT(INT, JSON_VALUE(@Body,'lax $.Position'));
IF (@Position IS NULL)
	RETURN;
DECLARE @OutputPosition INT = POWER(2, @Position);

DECLARE @Value NVARCHAR(250) = JSON_VALUE(@Body,'lax $.ValueRaw');
DECLARE @IsNullOperator BIT = TRY_CONVERT(BIT, JSON_VALUE(@Body,'lax $.IsNullOperator'));
DECLARE @IsEqual BIT = TRY_CONVERT(BIT, JSON_VALUE(@Body,'lax $.IsEqual'));

DECLARE @IsNotEqual BIT = TRY_CONVERT(BIT, JSON_VALUE(@Body,'lax $.IsNotEqual'));

DECLARE @IsNegation BIT = ISNULL(TRY_CONVERT(BIT, JSON_VALUE(@Body,'lax $.IsNegation')),0);

IF (@IsNullOperator = 1 AND (@IsNegation = 0 AND @IsEqual = 1) OR (@IsNegation = 1 AND @IsEqual = 0))
BEGIN
   WITH EntitySet(Id) AS(
   SELECT Id FROM [{NamespaceTable}].[{EntityName}]
   EXCEPT
   SELECT EntityId FROM [{NamespaceTable}].[{EntityName}Property] WHERE [KeyId] = @PropertyKey
   )
   INSERT @Results (Id, Position)
   SELECT Id, @OutputPosition FROM EntitySet
   RETURN;
END

IF (@IsNullOperator = 1 AND (@IsNegation = 0 AND @IsEqual = 0) OR (@IsNegation = 1 AND @IsEqual = 1))
BEGIN
   WITH EntitySet(Id) AS(
   SELECT DISTINCT EntityId FROM [{NamespaceTable}].[{EntityName}Property] WHERE [KeyId] = @PropertyKey
   )
   INSERT @Results (Id, Position)
   SELECT Id, @OutputPosition FROM EntitySet;
   RETURN;
END

IF (@IsNegation = 1)
	SET @Operator = CASE @Operator 
		WHEN 'eq' THEN 'ne' 
		WHEN 'ne' THEN 'eq' 
		WHEN 'lt' THEN 'ge' 
		WHEN 'le' THEN 'gt' 
		WHEN 'gt' THEN 'le' 
		WHEN 'ge' THEN 'lt' 
	END;

IF (@Operator = 'eq')
BEGIN
   WITH EntitySet(Id) AS(
   SELECT DISTINCT EntityId FROM [{NamespaceTable}].[{EntityName}Property] WHERE [KeyId] = @PropertyKey AND [Value]=@Value
   )
   INSERT @Results (Id, Position)
   SELECT Id, @OutputPosition FROM EntitySet;
   RETURN;
END

IF (@Operator = 'ne')
BEGIN
   WITH EntitySet(Id) AS(
   SELECT DISTINCT EntityId FROM [{NamespaceTable}].[{EntityName}Property] WHERE [KeyId] = @PropertyKey AND [Value]!=@Value
   )
   INSERT @Results (Id, Position)
   SELECT Id, @OutputPosition FROM EntitySet;
   RETURN;
END

IF (@Operator = 'lt')
BEGIN
   WITH EntitySet(Id) AS(
   SELECT DISTINCT EntityId FROM [{NamespaceTable}].[{EntityName}Property] WHERE [KeyId] = @PropertyKey AND [Value]<@Value
   )
   INSERT @Results (Id, Position)
   SELECT Id, @OutputPosition FROM EntitySet;
   RETURN;
END

IF (@Operator = 'le')
BEGIN
   WITH EntitySet(Id) AS(
   SELECT DISTINCT EntityId FROM [{NamespaceTable}].[{EntityName}Property] WHERE [KeyId] = @PropertyKey AND [Value]<=@Value
   )
   INSERT @Results (Id, Position)
   SELECT Id, @OutputPosition FROM EntitySet;
   RETURN;
END

IF (@Operator = 'gt')
BEGIN
   WITH EntitySet(Id) AS(
   SELECT DISTINCT EntityId FROM [{NamespaceTable}].[{EntityName}Property] WHERE [KeyId] = @PropertyKey AND [Value]>@Value
   )
   INSERT @Results (Id, Position)
   SELECT Id, @OutputPosition FROM EntitySet;
   RETURN;
END

IF (@Operator = 'ge')
BEGIN
   WITH EntitySet(Id) AS(
   SELECT DISTINCT EntityId FROM [{NamespaceTable}].[{EntityName}Property] WHERE [KeyId] = @PropertyKey AND [Value]>=@Value
   )
   INSERT @Results (Id, Position)
   SELECT Id, @OutputPosition FROM EntitySet;
   RETURN;
END

RETURN;
END;  