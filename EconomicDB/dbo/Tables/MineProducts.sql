CREATE TABLE [dbo].[MineProducts] (
    [MineName]    NVARCHAR (20) NOT NULL,
    [ProductName] NVARCHAR (20) NOT NULL,
    [Amount]      FLOAT (53)    NOT NULL,
    PRIMARY KEY CLUSTERED ([MineName] ASC),
    CONSTRAINT [FK_MineProducts_ToMines] FOREIGN KEY ([MineName]) REFERENCES [dbo].[Mines] ([Name]),
    CONSTRAINT [FK_MineProducts_ToProducts] FOREIGN KEY ([ProductName]) REFERENCES [dbo].[Products] ([Name])
);

