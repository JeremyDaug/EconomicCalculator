CREATE TABLE [dbo].[MarketCurrencies] (
    [MarketName]   NVARCHAR (20) NOT NULL,
    [CurrencyName] NVARCHAR (20) NOT NULL,
    PRIMARY KEY CLUSTERED ([CurrencyName] ASC, [MarketName] ASC),
    CONSTRAINT [FK_MarketCurrencies_ToCurrencies] FOREIGN KEY ([CurrencyName]) REFERENCES [dbo].[Currencies] ([Name]),
    CONSTRAINT [FK_MarketCurrencies_ToMarkets] FOREIGN KEY ([MarketName]) REFERENCES [dbo].[Markets] ([Name])
);

