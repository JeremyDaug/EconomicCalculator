CREATE TABLE [dbo].[Mines] (
    [Name]              NVARCHAR (20) NOT NULL,
    [MineType]        NVARCHAR(20)           NOT NULL,
    [RockType]        NVARCHAR(20)           NOT NULL,
    [LaborRequirements] FLOAT (53)    NOT NULL,
    PRIMARY KEY CLUSTERED ([Name] ASC)
);

