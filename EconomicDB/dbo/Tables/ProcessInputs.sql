CREATE TABLE [dbo].[ProcessInputs] (
    [ProcessName] NVARCHAR (20) NOT NULL,
    [InputName]   NVARCHAR (20) NOT NULL,
    [Amount]      FLOAT (53)    NOT NULL,
    PRIMARY KEY CLUSTERED ([ProcessName] ASC),
    CONSTRAINT [FK_ProcessInputs_ToProcesses] FOREIGN KEY ([ProcessName]) REFERENCES [dbo].[Processes] ([Name]),
    CONSTRAINT [FK_ProcessInputs_ToProducts] FOREIGN KEY ([InputName]) REFERENCES [dbo].[Products] ([Name])
);

