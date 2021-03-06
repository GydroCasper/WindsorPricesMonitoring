USE [WindsorMonitoring]
GO
/****** Object:  Table [dbo].[ApartmentTypeAvailability]    Script Date: 6/17/2021 6:34:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ApartmentTypeAvailability](
	[Id] [uniqueidentifier] NOT NULL,
	[Date] [date] NOT NULL,
	[Available] [tinyint] NOT NULL,
	[TypeId] [uniqueidentifier] NOT NULL,
	[MinimumPrice] [smallint] NULL,
 CONSTRAINT [PK_ApartmentTypeAvailability] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_ApartmentTypeAvailability] UNIQUE NONCLUSTERED 
(
	[Date] DESC,
	[TypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ApartmentTypeAvailability] ADD  CONSTRAINT [DF_ApartmentTypeAvailability_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[ApartmentTypeAvailability] ADD  CONSTRAINT [DF_ApartmentTypeAvailability_Available]  DEFAULT ((0)) FOR [Available]
GO
ALTER TABLE [dbo].[ApartmentTypeAvailability]  WITH CHECK ADD  CONSTRAINT [FK_ApartmentTypeAvailability_ApartamentTypes] FOREIGN KEY([TypeId])
REFERENCES [dbo].[ApartmentTypes] ([Id])
GO
ALTER TABLE [dbo].[ApartmentTypeAvailability] CHECK CONSTRAINT [FK_ApartmentTypeAvailability_ApartamentTypes]
GO
