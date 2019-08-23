--#region.extension
CREATE TABLE [dbo].[Test1_Extension]
(
     [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
     [EntityId] BIGINT NOT NULL,
	 [AccountId] UNIQUEIDENTIFIER NULL,
	 [Second] INT NULL,
	 CONSTRAINT [FK_Test1_Extension_EntityId] FOREIGN KEY ([EntityId]) REFERENCES [dbo].[Test1]([Id]), 
)
GO
CREATE UNIQUE INDEX[IX_Test1_Extension_EntityId] ON [dbo].[Test1_Extension] ([EntityId])
GO
--#endregion
GO
--#region.extension
CREATE VIEW [dbo].[ViewTest1]
AS
	SELECT E.*, EX.AccountId, EX.[Second]
	FROM [dbo].[Test1] AS E 
	INNER JOIN [dbo].[Test1_Extension] AS EX ON E.Id = EX.EntityId
GO
--#endregion
GO