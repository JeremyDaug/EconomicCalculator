CREATE TABLE [dbo].[MarketMines] (
    [MarketName] NVARCHAR (20) NOT NULL,
    [MineName]   NVARCHAR (20) NOT NULL,
    PRIMARY KEY CLUSTERED ([MarketName] ASC, [MineName] ASC),
    CONSTRAINT [FK_MarketMines_ToMarkets] FOREIGN KEY ([MarketName]) REFERENCES [dbo].[Markets] ([Name]),
    CONSTRAINT [FK_MarketMines_ToMines] FOREIGN KEY ([MineName]) REFERENCES [dbo].[Mines] ([Name])
);

