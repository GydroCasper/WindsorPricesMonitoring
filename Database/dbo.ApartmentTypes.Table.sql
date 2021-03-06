USE [WindsorMonitoring]
GO
/****** Object:  Table [dbo].[ApartmentTypes]    Script Date: 6/17/2021 6:34:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ApartmentTypes](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](10) NOT NULL,
	[BedsCount] [tinyint] NOT NULL,
	[BathsCount] [tinyint] NOT NULL,
 CONSTRAINT [PK_ApartamentTypes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_ApartamentTypes] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ApartmentTypes] ADD  CONSTRAINT [DF_ApartamentTypes_Id]  DEFAULT (newid()) FOR [Id]
GO
