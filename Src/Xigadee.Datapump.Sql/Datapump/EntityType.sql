CREATE TABLE [Datapump].[EntityType]
(
	 [Id] SMALLINT NOT NULL PRIMARY KEY IDENTITY(1,1)
	,[Name] VARCHAR(250) NOT NULL
)
GO
CREATE UNIQUE INDEX [IX_EntityType_Name] ON [Datapump].[EntityType]([Name]) INCLUDE ([Id])
GO