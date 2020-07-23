CREATE TABLE [dbo].[MarketPopulations] (
    [MarketName]     NVARCHAR (20) NOT NULL,
    [PopulationName] NVARCHAR (20) NOT NULL,
    PRIMARY KEY CLUSTERED ([MarketName] ASC, [PopulationName] ASC),
    CONSTRAINT [FK_MarketPopulations_ToMarket] FOREIGN KEY ([MarketName]) REFERENCES [dbo].[Markets] ([Name]),
    CONSTRAINT [FK_MarketPopulations_ToPopulations] FOREIGN KEY ([PopulationName]) REFERENCES [dbo].[Populations] ([Name])
);

