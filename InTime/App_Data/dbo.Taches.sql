CREATE TABLE [dbo].[Taches] (
    [IdTache]     INT           IDENTITY (1, 1) NOT NULL,
    [UserId]      INT           NOT NULL,
    [NomTache]    VARCHAR (50)  NOT NULL,
    [Lieu]        VARCHAR (50)  NOT NULL,
    [Description] VARCHAR (MAX) NULL,
    [Mois]        NVARCHAR (10) NOT NULL,
    [Jour]        VARCHAR (2)   NOT NULL,
    [HDebut]      VARCHAR (2)   NOT NULL,
    [HFin]        VARCHAR (2)   NOT NULL,
    [mDebut]      VARCHAR (2)   NOT NULL,
    [mFin]        VARCHAR (2)   NOT NULL,
    [HRappel]     VARCHAR (2)   NULL,
    [mRappel]     VARCHAR (2)   NULL,
    [Annee] VARCHAR(4) NOT NULL, 
    PRIMARY KEY CLUSTERED ([IdTache] ASC),
    CONSTRAINT [FK_UserProfile_Taches] FOREIGN KEY ([UserId]) REFERENCES [dbo].[UserProfile] ([UserId])
);
