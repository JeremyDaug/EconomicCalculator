CREATE TABLE [dbo].[PopulationMoney] (
    [PopulationName] NVARCHAR (20) NOT NULL,
    [CurrencyName]   NVARCHAR (20) NOT NULL,
    [Amount]         NCHAR (10)    NOT NULL,
    PRIMARY KEY CLUSTERED ([PopulationName] ASC, [CurrencyName] ASC),
    CONSTRAINT [FK_Table_ToCurrencies] FOREIGN KEY ([CurrencyName]) REFERENCES [dbo].[Currencies] ([Name]),
    CONSTRAINT [FK_Table_ToPopulations] FOREIGN KEY ([PopulationName]) REFERENCES [dbo].[Populations] ([Name])
);

