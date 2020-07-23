CREATE TABLE [dbo].[CropOutputs] (
    [CropName]      NVARCHAR (20) NOT NULL,
    [OutputProduct] NVARCHAR (20) NOT NULL,
    [Amount]        FLOAT (53)    NOT NULL,
    PRIMARY KEY CLUSTERED ([OutputProduct] ASC, [CropName] ASC),
    CONSTRAINT [FK_CropOutputs_ToCrops] FOREIGN KEY ([CropName]) REFERENCES [dbo].[Crops] ([Name]),
    CONSTRAINT [FK_CropOutputs_ToProducts] FOREIGN KEY ([OutputProduct]) REFERENCES [dbo].[Products] ([Name])
);

