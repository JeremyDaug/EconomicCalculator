CREATE TABLE [dbo].[MarketCrops] (
    [MarketName] NVARCHAR (20) NOT NULL,
    [CropName]   NVARCHAR (20) NOT NULL,
    PRIMARY KEY CLUSTERED ([CropName] ASC, [MarketName] ASC),
    CONSTRAINT [FK_MarketCrops_ToCrops] FOREIGN KEY ([CropName]) REFERENCES [dbo].[Crops] ([Name]),
    CONSTRAINT [FK_MarketCrops_ToMarkets] FOREIGN KEY ([MarketName]) REFERENCES [dbo].[Markets] ([Name])
);

