CREATE TABLE [dbo].[ProcessOutputs] (
    [ProcessName] NVARCHAR (20) NOT NULL,
    [OutputName]  NVARCHAR (20) NOT NULL,
    [Amount]      FLOAT (53)    NULL,
    PRIMARY KEY CLUSTERED ([ProcessName], [OutputName]),
    CONSTRAINT [FK_ProcessOutputs_ToProcess] FOREIGN KEY ([ProcessName]) REFERENCES [dbo].[Processes] ([Name]),
    CONSTRAINT [FK_ProcessOutputs_ToProducts] FOREIGN KEY ([OutputName]) REFERENCES [dbo].[Products] ([Name])
);

