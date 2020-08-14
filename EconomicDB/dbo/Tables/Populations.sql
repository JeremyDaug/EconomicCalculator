CREATE TABLE [dbo].[Populations] (
    [Name]  NVARCHAR (20) NOT NULL,
	[Variant] NVARCHAR (20) NOT NULL,
	[Market] NVARCHAR(20) NOT NULL, 
    [Count] INT    NOT NULL,
    [JobCategory]   NVARCHAR (20) NOT NULL,
    [JobName] NVARCHAR(20) NULL, 
    CONSTRAINT [FK_Populations_JobBoard] FOREIGN KEY ([JobName]) REFERENCES [JobBoard]([JobName]), 
    CONSTRAINT [FK_Populations_Market] FOREIGN KEY ([Market]) REFERENCES [Markets]([Name]), 
    CONSTRAINT [PK_Populations] PRIMARY KEY ([Name], [Variant])
);

