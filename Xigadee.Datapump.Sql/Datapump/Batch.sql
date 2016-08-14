CREATE TABLE [Datapump].[Batch]
(
	 [Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1)
	,[BatchId] VARCHAR(50)
	,[DateFirstRecorded] DATETIME NOT NULL DEFAULT(GETUTCDATE()) 
)
GO
CREATE UNIQUE INDEX [IX_Batch_BatchId] ON [Datapump].[Batch] ([BatchId]) INCLUDE ([Id])
GO
