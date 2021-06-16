USE [WindsorMonitoring]
GO
/****** Object:  View [dbo].[LastApartmentAvailability]    Script Date: 6/16/2021 4:57:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[LastApartmentAvailability]
AS
SELECT a.Id, a.TypeId, t.[Name] AS [Type], a.[Date], a.Available, a.MinimumPrice
FROM dbo.ApartmentTypeAvailability a
INNER JOIN (SELECT TypeId, MAX([Date]) AS LastDate FROM dbo.ApartmentTypeAvailability GROUP BY TypeId) sel ON sel.TypeId = a.TypeId AND a.[Date] = sel.LastDate
INNER JOIN dbo.ApartmentTypes t ON t.Id = a.TypeId
GO
