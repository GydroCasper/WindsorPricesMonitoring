USE [WindsorMonitoring]
GO
/****** Object:  View [dbo].[LastUnitAvailability]    Script Date: 6/17/2021 6:34:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[LastUnitAvailability]
AS

SELECT a.Id, a.IndividualApartmentTypeId AS TypeId, t.FullNumber, a.[Date], a.DateAvailable, a.IsAvailable, a.MinimumPrice, aptt.BedsCount, t.BuildingNumber, t.UnitNumber, aptt.Name AS ApartmentType
	FROM dbo.IndividualApartmentAvailability a
	INNER JOIN (SELECT IndividualApartmentTypeId, MAX([Date]) AS LastDate FROM dbo.IndividualApartmentAvailability GROUP BY IndividualApartmentTypeId) sel ON sel.IndividualApartmentTypeId = a.IndividualApartmentTypeId AND a.[Date] = sel.LastDate
	INNER JOIN dbo.IndividualApartmentTypes t ON t.Id = a.IndividualApartmentTypeId
	INNER JOIN dbo.ApartmentTypes aptt ON aptt.Id = t.TypeId


GO
