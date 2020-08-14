CREATE TABLE [dbo].[PopulationGoods] (
    [PopulationName] NVARCHAR (20) NOT NULL,
	[VariantName] NVARCHAR (20) NOT NULL,
    [ProductName]    NVARCHAR (20) NOT NULL,
    [Amount]         FLOAT (53)    NOT NULL,
    CONSTRAINT [FK_PopulationGoods_ToPopulations] FOREIGN KEY ([PopulationName], [VariantName]) REFERENCES [dbo].[Populations] ([Name], [Variant]),
    CONSTRAINT [FK_PopulationGoods_ToProducts] FOREIGN KEY ([ProductName]) REFERENCES [dbo].[Products] ([Name]), 
    CONSTRAINT [PK_PopulationGoods] PRIMARY KEY ([ProductName], [PopulationName], [VariantName])
);

