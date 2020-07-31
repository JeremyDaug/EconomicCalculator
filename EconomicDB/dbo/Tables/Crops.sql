CREATE TABLE [dbo].[Crops] (
    [Name]      NVARCHAR (20) NOT NULL,
    [Type]      NCHAR (10)    NOT NULL,
    [Planting]  FLOAT    NOT NULL,
    [Seed]      NVARCHAR (20) NOT NULL,
    [Labor]     FLOAT (53)    NOT NULL,
    [LifeCycle] INT           NOT NULL,
    PRIMARY KEY CLUSTERED ([Name] ASC),
    CONSTRAINT [FK_Crops_SeedProducts] FOREIGN KEY ([Seed]) REFERENCES [dbo].[Products] ([Name])
);

