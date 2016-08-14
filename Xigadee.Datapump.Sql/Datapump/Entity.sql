CREATE TABLE [Datapump].[Entity]
(
	 [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1)
	,[EntityType] SMALLINT NOT NULL
	,[Key] NVARCHAR(255) NOT NULL
	,[DateFirstRecorded] DATETIME NULL DEFAULT(GETUTCDATE()) 
    CONSTRAINT [FK_Entity_ToEntityType] FOREIGN KEY ([EntityType]) REFERENCES [Datapump].[EntityType]([Id])
)
GO
CREATE UNIQUE INDEX [IX_Entity_Unique] ON [Datapump].[Entity] ([EntityType],[Key])
GO
