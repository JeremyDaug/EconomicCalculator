CREATE TABLE [dbo].[Products]
(
	[Name] NVARCHAR(20) NOT NULL PRIMARY KEY, 
    [UnitId] INT NOT NULL, 
    [CurrentPrice] FLOAT NOT NULL, 
    [MeanTimeToFailure] FLOAT NOT NULL, 
    CONSTRAINT [FK_Products_ToUnits] FOREIGN KEY ([UnitId]) REFERENCES [Units]([Id])
)
