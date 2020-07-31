CREATE TABLE [dbo].[Products] (
    [Name]              NVARCHAR (20) NOT NULL,
    [UnitId]            INT           NOT NULL,
    [CurrentPrice]      FLOAT (53)    NOT NULL,
    [MeanTimeToFailure] INT    NOT NULL,
    PRIMARY KEY CLUSTERED ([Name] ASC),
    CONSTRAINT [FK_Products_ToUnits] FOREIGN KEY ([UnitId]) REFERENCES [dbo].[Units] ([UnitId])
);


