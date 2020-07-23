CREATE TABLE [dbo].[Mines] (
    [Name]              NVARCHAR (20) NOT NULL,
    [MineTypeId]        INT           NOT NULL,
    [RockTypeId]        INT           NOT NULL,
    [LaborRequirements] FLOAT (53)    NOT NULL,
    PRIMARY KEY CLUSTERED ([Name] ASC),
    CONSTRAINT [FK_Mines_ToMineTypes] FOREIGN KEY ([MineTypeId]) REFERENCES [dbo].[MineTypes] ([Id]),
    CONSTRAINT [FK_Mines_ToRockTypes] FOREIGN KEY ([RockTypeId]) REFERENCES [dbo].[RockTypes] ([Id])
);

