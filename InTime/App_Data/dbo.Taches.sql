CREATE TABLE [dbo].[Taches] (
    [IdTache]     INT           IDENTITY (1, 1) NOT NULL,
    [UserId]      INT           NOT NULL,
    [NomTache]    VARCHAR (50)  NOT NULL,
    [Lieu]        VARCHAR (50)  NOT NULL,
    [Description] VARCHAR (MAX) NULL,
    [DateDebut]   NUMERIC(10,0) NOT NULL,
    [DateFin]     NUMERIC(10,0) NOT NULL,
    [HRappel]     VARCHAR (2)   NULL,
    [mRappel]     VARCHAR (2)   NULL,
    [Reccurence]  NUMERIC (2,0)  NULL,  
    PRIMARY KEY CLUSTERED ([IdTache] ASC),
    CONSTRAINT [FK_UserProfile_Taches] FOREIGN KEY ([UserId]) REFERENCES [dbo].[UserProfile] ([UserId])
);

