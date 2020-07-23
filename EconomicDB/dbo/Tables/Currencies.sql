CREATE TABLE [dbo].[Currencies] (
    [Name]    NVARCHAR (20) NOT NULL,
    [Cash]    NVARCHAR (20) NOT NULL,
    [Backing] NVARCHAR (20) NULL,
    [Value]   FLOAT (53)    NOT NULL,
    PRIMARY KEY CLUSTERED ([Name] ASC),
    CONSTRAINT [FK_Currencies_BackingProduct] FOREIGN KEY ([Backing]) REFERENCES [dbo].[Products] ([Name]),
    CONSTRAINT [FK_Currencies_CashProduct] FOREIGN KEY ([Cash]) REFERENCES [dbo].[Products] ([Name])
);

