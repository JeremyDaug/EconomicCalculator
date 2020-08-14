CREATE TABLE [dbo].[Products] (
    [Name]              NVARCHAR (20) NOT NULL,
    [Unit]            NVARCHAR(20)           NOT NULL,
    [CurrentPrice]      FLOAT (53)    NOT NULL,
    [MeanTimeToFailure] INT    NOT NULL,
    PRIMARY KEY CLUSTERED ([Name] ASC),
);


