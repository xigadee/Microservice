CREATE TABLE [Datapump].[Instance]
(
	 [Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1)
	,[Name] NVARCHAR(250) NOT NULL
	,[DateFirstRecorded] DATETIME NOT NULL DEFAULT(GETUTCDATE()) 
)
GO
CREATE UNIQUE INDEX [IX_Instance_Name] ON [Datapump].[Instance] ([Name]) INCLUDE ([Id])
GO
