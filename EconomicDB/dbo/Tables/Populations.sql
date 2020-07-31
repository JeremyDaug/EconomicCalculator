CREATE TABLE [dbo].[Populations] (
    [Name]  NVARCHAR (20) NOT NULL,
    [Count] INT    NOT NULL,
    [JobCategory]   NVARCHAR (20) NOT NULL,
    [JobName] NVARCHAR(20) NULL, 
    PRIMARY KEY CLUSTERED ([Name] ASC), 
    CONSTRAINT [FK_Populations_JobBoard] FOREIGN KEY ([JobName]) REFERENCES [JobBoard]([JobName])
);

