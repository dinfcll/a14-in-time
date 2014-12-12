CREATE TABLE [dbo].[UserProfile] (
	[UserId]   INT IDENTITY (1, 1) NOT NULL,
    [UserName] NVARCHAR (56) NOT NULL,
    [Nom] VARCHAR(50) NOT NULL, 
    [Prenom] VARCHAR(50) NOT NULL, 
    [Email] VARCHAR(50) NOT NULL, 
	[Categorie] VARCHAR(50) NOT NULL,
    PRIMARY KEY CLUSTERED ([UserId] ASC),
    UNIQUE NONCLUSTERED ([Email],[UserName] ASC)
);

