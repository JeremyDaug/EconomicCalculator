CREATE TABLE [dbo].[Populations] (
    [Name]  NVARCHAR (20) NOT NULL,
    [Count] FLOAT (53)    NOT NULL,
    [Job]   NVARCHAR (20) NOT NULL,
    PRIMARY KEY CLUSTERED ([Name] ASC)
);

