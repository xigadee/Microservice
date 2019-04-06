-- Schema that will be used to grant external access to the DB
CREATE SCHEMA [External]

GO
CREATE TYPE [External].[KvpTableType] AS TABLE
(
	RefType VARCHAR(30), 
	RefValue NVARCHAR(256)
)

GO
CREATE TYPE [External].[IdTableType] AS TABLE
(
	ExternalId UNIQUEIDENTIFIER
)

GO
