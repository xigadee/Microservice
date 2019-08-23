--#region.extension
CREATE TABLE [dbo].[Account_Extension]
(
     [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
     [EntityId] BIGINT NOT NULL,
	 [Type] INT NULL,
	 [GroupId] UNIQUEIDENTIFIER NULL,
	 [UserId] BIGINT NULL,
	 CONSTRAINT [FK_Account_Extension_EntityId] FOREIGN KEY ([EntityId]) REFERENCES [dbo].[Account]([Id]), 
	 CONSTRAINT [FK_Account_Extension_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User]([Id]), 
)
GO
CREATE UNIQUE INDEX[IX_Account_Extension_EntityId] ON [dbo].[Account_Extension] ([EntityId])
GO
--#endregion
GO
--#region.extension
CREATE VIEW [dbo].[ViewAccount]
AS
	SELECT E.[Id] 
	,E.[ExternalId] 
    ,E.[VersionId] 
    ,E.[UserIdAudit] 
	,E.[DateCreated] 
	,E.[DateUpdated] 
	,E.[Sig] 
	,E.[Body]
	,EX.[Type]
	,EX.[GroupId]
	,U.ExternalId AS [UserId]
	FROM [dbo].[Account] AS E 
	INNER JOIN [dbo].[Account_Extension] AS EX ON E.Id = EX.EntityId
	LEFT JOIN [dbo].[User] AS U ON U.Id = EX.[UserId]
GO
--#endregion
GO
