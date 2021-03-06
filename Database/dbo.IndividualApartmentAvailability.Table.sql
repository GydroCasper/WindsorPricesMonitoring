USE [WindsorMonitoring]
GO
/****** Object:  Table [dbo].[IndividualApartmentAvailability]    Script Date: 6/17/2021 6:34:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[IndividualApartmentAvailability](
	[Id] [uniqueidentifier] NOT NULL,
	[IndividualApartmentTypeId] [uniqueidentifier] NOT NULL,
	[MinimumPrice] [smallint] NULL,
	[DateAvailable] [date] NULL,
	[IsAvailable] [bit] NOT NULL,
	[Date] [date] NOT NULL,
 CONSTRAINT [PK_IndividualApparmentAvailability] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_IndividualApartmentAvailability] UNIQUE NONCLUSTERED 
(
	[Date] DESC,
	[IndividualApartmentTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[IndividualApartmentAvailability] ADD  CONSTRAINT [DF_IndividualApparmentAvailability_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[IndividualApartmentAvailability]  WITH CHECK ADD  CONSTRAINT [FK_IndividualApparmentAvailability_IndividualApartmentTypes] FOREIGN KEY([IndividualApartmentTypeId])
REFERENCES [dbo].[IndividualApartmentTypes] ([Id])
GO
ALTER TABLE [dbo].[IndividualApartmentAvailability] CHECK CONSTRAINT [FK_IndividualApparmentAvailability_IndividualApartmentTypes]
GO
