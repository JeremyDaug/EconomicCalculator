CREATE TABLE [dbo].[PopulationLifeNeeds] (
    [PopulationName] NVARCHAR (20) NOT NULL,
	[VariantName]    NVARCHAR (20) NOT NULL,
    [ProductName]    NVARCHAR (20) NOT NULL,
    [Amount]         FLOAT (53)    NOT NULL,
    CONSTRAINT [FK_PopulationLifeNeeds_ToPopulations] FOREIGN KEY ([PopulationName], [VariantName]) REFERENCES [dbo].[Populations] ([Name], [Variant]),
    CONSTRAINT [FK_PopulationLifeNeeds_ToProducts] FOREIGN KEY ([ProductName]) REFERENCES [dbo].[Products] ([Name]), 
    CONSTRAINT [PK_PopulationLifeNeeds] PRIMARY KEY ([ProductName], [PopulationName], [VariantName])
);

