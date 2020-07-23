CREATE TABLE [dbo].[RockTypes] (
    [Id]   INT           NOT NULL,
    [Name] NVARCHAR (20) NOT NULL,
    CONSTRAINT [PK_RockTypes] PRIMARY KEY CLUSTERED ([Id] ASC),
    UNIQUE NONCLUSTERED ([Name] ASC)
);

