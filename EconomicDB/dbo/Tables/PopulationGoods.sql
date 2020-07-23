CREATE TABLE [dbo].[PopulationGoods] (
    [PopulationName] NVARCHAR (20) NOT NULL,
    [ProductName]    NVARCHAR (20) NOT NULL,
    [Amount]         FLOAT (53)    NOT NULL,
    PRIMARY KEY CLUSTERED ([ProductName] ASC, [PopulationName] ASC),
    CONSTRAINT [FK_PopulationGoods_ToPopulations] FOREIGN KEY ([PopulationName]) REFERENCES [dbo].[Populations] ([Name]),
    CONSTRAINT [FK_PopulationGoods_ToProducts] FOREIGN KEY ([ProductName]) REFERENCES [dbo].[Products] ([Name])
);

