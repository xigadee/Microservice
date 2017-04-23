CREATE TABLE [Datapump].[EventType]
(
	 [Id] TINYINT NOT NULL PRIMARY KEY IDENTITY(1,1)
	,[Name] VARCHAR(50) NOT NULL
)
GO
CREATE UNIQUE INDEX [IX_EventType_Name] ON [Datapump].[EventType]([Name]) INCLUDE ([Id])
GO
