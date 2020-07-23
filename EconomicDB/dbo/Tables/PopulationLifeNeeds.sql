CREATE TABLE [dbo].[PopulationLifeNeeds] (
    [PopulationName] NVARCHAR (20) NOT NULL,
    [ProductName]    NVARCHAR (20) NOT NULL,
    [Amount]         FLOAT (53)    NOT NULL,
    PRIMARY KEY CLUSTERED ([ProductName] ASC, [PopulationName] ASC),
    CONSTRAINT [FK_PopulationLifeNeeds_ToPopulations] FOREIGN KEY ([PopulationName]) REFERENCES [dbo].[Populations] ([Name]),
    CONSTRAINT [FK_PopulationLifeNeeds_ToProducts] FOREIGN KEY ([ProductName]) REFERENCES [dbo].[Products] ([Name])
);

