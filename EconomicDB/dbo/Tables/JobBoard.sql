CREATE TABLE [dbo].[JobBoard]
(
	[JobName] NVARCHAR(20) NOT NULL PRIMARY KEY, 
    [CropJob] NVARCHAR(20) NULL, 
    [MineJob] NVARCHAR(20) NULL, 
    [ProcessJob] NVARCHAR(20) NULL, 
    CONSTRAINT [CK_JobBoard_NeedJob] CHECK (
		Case when CropJob IS Null Then 0 Else 1 End +
		Case when MineJob Is Null Then 0 Else 1 End +
		Case when ProcessJob Is Null Then 0 Else 1 End = 1
	), 
    CONSTRAINT [FK_JobBoard_ToCrops] FOREIGN KEY ([CropJob]) REFERENCES [Crops]([Name]), 
    CONSTRAINT [FK_JobBoard_ToMines] FOREIGN KEY ([MineJob]) REFERENCES [Mines]([Name]), 
    CONSTRAINT [FK_JobBoard_ToProcesses] FOREIGN KEY ([ProcessJob]) REFERENCES [Processes]([Name])
)
