CREATE TABLE [Boundary].[Log]
(
	[Id] BIGINT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	[TimeStamp] DATETIME2 NOT NULL DEFAULT(SYSUTCDATETIME()),
	[ServiceName] VARCHAR(50) NOT NULL,
	[ServiceId] VARCHAR(50) NOT NULL,
	[PayloadId] UNIQUEIDENTIFIER NOT NULL,
	[Direction] BIT NOT NULL,

	[ChannelId] VARCHAR(50),
	[ChannelPriority] INT,
	[MessageType] VARCHAR(50),
	[ActionType] VARCHAR(50),

	[OriginatorKey] VARCHAR(250),
	[OriginatorServiceId] VARCHAR(250),
	[OriginatorUTC] DATETIME2,

	[CorrelationKey] VARCHAR(250),
	[CorrelationServiceId] VARCHAR(250),
	[CorrelationUTC] DATETIME2,

	[ResponseChannelId] VARCHAR(250),
	[ResponseChannelPriority] INT,

	[Status] VARCHAR(250),
	[StatusDescription] VARCHAR(250), 
    [BatchId] UNIQUEIDENTIFIER NULL,

)
