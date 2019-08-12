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
CREATE TABLE [dbo].[SearchHistory]
(
     [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1)
	,[ETag] UNIQUEIDENTIFIER NOT NULL
	,[EntityType] VARCHAR(50)
	,[SearchType] VARCHAR(50)
	,[TimeStamp] DATETIME NOT NULL DEFAULT(GETUTCDATE())
	,[Sig] VARCHAR(256) NULL
	,[Body] NVARCHAR(MAX) NULL
)
GO 
CREATE UNIQUE INDEX[IX_SearchHistory_ETag] ON [dbo].[SearchHistory] ([ETag]) 

GO
CREATE PROCEDURE [dbo].spSearchLog
	 @ETag UNIQUEIDENTIFIER
	,@EntityType VARCHAR(50)
	,@SearchType VARCHAR(50)
	,@Body NVARCHAR(MAX)
AS
BEGIN

INSERT INTO [dbo].[SearchHistory]
([ETag],[EntityType],[SearchType],[Sig],[Body])
VALUES
(@ETag, @EntityType, @SearchType, '', @Body);


END
GO
