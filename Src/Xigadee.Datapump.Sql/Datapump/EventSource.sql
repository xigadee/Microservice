CREATE TABLE [Datapump].[EventSource]
(
	 [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1)
	,[EventType] TINYINT NOT NULL
    ,[EntityType] SMALLINT NOT NULL
    ,[Instance] INT NOT NULL
    ,[BatchId] INT NULL
    ,[Entity] BIGINT NOT NULL
	,[UTCTimeStamp] DATETIME NOT NULL DEFAULT (getutcdate())
	,[Version] VARCHAR(50) NULL
	,[CorrelationId] VARCHAR(250) NULL

	,[IdOld] BIGINT NULL
    ,[IdPrevious] BIGINT NULL
	,[Source] VARCHAR(50) NULL
	,[SourceId] VARCHAR(50) NULL
	,[SourceName] VARCHAR(50) NULL

    ,CONSTRAINT [FK_EventSource_ToEventType] FOREIGN KEY ([EventType]) REFERENCES [Datapump].[EventType]([Id])
    ,CONSTRAINT [FK_EventSource_ToEntityType] FOREIGN KEY ([EntityType]) REFERENCES [Datapump].[EntityType]([Id])
    ,CONSTRAINT [FK_EventSource_ToEntity] FOREIGN KEY ([Entity]) REFERENCES [Datapump].[Entity]([Id])
    ,CONSTRAINT [FK_EventSource_ToInstance] FOREIGN KEY ([Instance]) REFERENCES [Datapump].[Instance]([Id])
    ,CONSTRAINT [FK_EventSource_ToBatch] FOREIGN KEY ([BatchId]) REFERENCES [Core].[Batch]([Id])
)

GO
CREATE INDEX [IX_EventSource_EntityVersion] ON [Datapump].[EventSource] ([Entity],[Version]) 
GO
CREATE INDEX [IX_EventSource_Date] ON [Datapump].[EventSource] ([UTCTimeStamp]) 
GO
CREATE INDEX [IX_EventSource_Entity] ON [Datapump].[EventSource] ([Entity]) 
GO
CREATE INDEX [IX_EventSource_Batch] ON [Datapump].[EventSource] ([BatchId]) 
GO
CREATE INDEX [IX_EventSource_EntityType] ON [Datapump].[EventSource] ([EntityType]) 
GO
CREATE UNIQUE INDEX [IX_EventSource_IdOld] ON [Datapump].[EventSource] ([IdOld]) WHERE [IdOld] IS NOT NULL
