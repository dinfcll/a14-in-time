CREATE TABLE [dbo].[DescTacheReccu] (
	[NouvIdTache]	INT				IDENTITY(1,1) NOT NULL,
	[IdTache]		INT				NOT NULL,
	[DateDebut]		NUMERIC(10,0)	NOT NULL,
	[DateFin]		NUMERIC(10,0)	NOT NULL,
	[Description]	VARCHAR(MAX)	NULL,
	PRIMARY KEY CLUSTERED (NouvIdTache),
	CONSTRAINT [FK_Taches_DescTacheReccu] FOREIGN KEY ([IdTache]) REFERENCES [dbo].[Taches] ([IdTache])
	ON DELETE CASCADE
	);
