CREATE TABLE [dbo].[MarketProcesses] (
    [MarketName]  NVARCHAR (20) NOT NULL,
    [ProcessName] NVARCHAR (20) NOT NULL,
    PRIMARY KEY CLUSTERED ([ProcessName] ASC, [MarketName] ASC),
    CONSTRAINT [FK_MarketProcesses_ToMarkets] FOREIGN KEY ([MarketName]) REFERENCES [dbo].[Markets] ([Name]),
    CONSTRAINT [FK_MarketProcesses_ToProcesses] FOREIGN KEY ([ProcessName]) REFERENCES [dbo].[Processes] ([Name])
);

