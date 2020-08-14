CREATE TABLE [dbo].[PopulationMoney] (
    [PopulationName] NVARCHAR (20) NOT NULL,
	[VariantName]    NVARCHAR (20) NOT NULL,
    [CurrencyName]   NVARCHAR (20) NOT NULL,
    [Amount]         FLOAT    NOT NULL,
    PRIMARY KEY CLUSTERED ([CurrencyName], [PopulationName], [VariantName]),
    CONSTRAINT [FK_Table_ToCurrencies] FOREIGN KEY ([CurrencyName]) REFERENCES [dbo].[Currencies] ([Name]),
    CONSTRAINT [FK_Table_ToPopulations] FOREIGN KEY ([PopulationName], [VariantName]) REFERENCES [dbo].[Populations] ([Name], [Variant]), 
);

