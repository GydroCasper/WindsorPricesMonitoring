USE [WindsorMonitoring]
GO
/****** Object:  Table [dbo].[IndividualApartmentTypes]    Script Date: 6/17/2021 6:34:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[IndividualApartmentTypes](
	[Id] [uniqueidentifier] NOT NULL,
	[FullNumber] [nvarchar](50) NOT NULL,
	[Sqft] [smallint] NOT NULL,
	[TypeId] [uniqueidentifier] NOT NULL,
	[Sqm]  AS ([Sqft]*(0.093)),
	[BuildingNumber]  AS (CONVERT([tinyint],substring([FullNumber],(1),(2)))),
	[UnitNumber]  AS (CONVERT([smallint],substring([FullNumber],(3),len([FullNumber])-(2)))),
 CONSTRAINT [PK_IndividualApartmentTypes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[IndividualApartmentTypes] ADD  CONSTRAINT [DF_IndividualApartmentTypes_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[IndividualApartmentTypes]  WITH CHECK ADD  CONSTRAINT [FK_IndividualApartmentTypes_ApartmentTypes] FOREIGN KEY([TypeId])
REFERENCES [dbo].[ApartmentTypes] ([Id])
GO
ALTER TABLE [dbo].[IndividualApartmentTypes] CHECK CONSTRAINT [FK_IndividualApartmentTypes_ApartmentTypes]
GO
