CREATE TABLE [dbo].[MineTypes] (
    [Id]   INT           NOT NULL,
    [Name] NVARCHAR (20) NOT NULL,
    CONSTRAINT [PK_MineTypes] PRIMARY KEY CLUSTERED ([Id] ASC),
    UNIQUE NONCLUSTERED ([Name] ASC)
);

