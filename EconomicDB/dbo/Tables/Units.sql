CREATE TABLE [dbo].[Units] (
    [UnitId] INT           NOT NULL,
    [Name]   NVARCHAR (20) NOT NULL,
    PRIMARY KEY CLUSTERED ([UnitId] ASC),
    UNIQUE NONCLUSTERED ([Name] ASC)
);


