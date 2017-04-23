CREATE TABLE [Datapump].[EventSourceData]
(
	 [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1)
	,[IdParent] BIGINT NOT NULL
    ,[CanRemove] BIT NOT NULL DEFAULT 0
	,[Data] NVARCHAR(MAX) NOT NULL
    CONSTRAINT [FK_EventSourceData_ToEventSource] FOREIGN KEY ([IdParent]) REFERENCES [Datapump].[EventSource]([Id])
)
GO

CREATE UNIQUE INDEX [IX_EventSourceData_Parent] ON [Datapump].[EventSourceData] ([IdParent]) 

GO

CREATE INDEX [IX_EventSourceData_CanRemove] ON [Datapump].[EventSourceData] ([CanRemove]) INCLUDE ([Id]) WHERE [CanRemove]=1
