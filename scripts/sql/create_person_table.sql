USE [LinqTest]
GO
/****** Object:  Table [dbo].[Person]    Script Date: 11/26/2008 20:15:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Person](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Version] [timestamp] NOT NULL,
	[Name] [nvarchar](10) COLLATE Latin1_General_CI_AS NULL,
	[Beruf] [nvarchar](20) COLLATE Latin1_General_CI_AS NULL,
 CONSTRAINT [PK_dbo.Person] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
