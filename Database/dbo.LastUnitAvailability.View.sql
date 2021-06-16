USE [WindsorMonitoring]
GO
/****** Object:  View [dbo].[LastUnitAvailability]    Script Date: 6/16/2021 4:57:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[LastUnitAvailability]
AS
	SELECT a.Id, a.IndividualApartmentTypeId AS TypeId, t.FullNumber, a.[Date], a.DateAvailable, a.IsAvailable, a.MinimumPrice
	FROM dbo.IndividualApartmentAvailability a
	INNER JOIN (SELECT IndividualApartmentTypeId, MAX(DateAvailable) AS LastDate FROM dbo.IndividualApartmentAvailability GROUP BY IndividualApartmentTypeId) sel ON sel.IndividualApartmentTypeId = a.IndividualApartmentTypeId AND a.DateAvailable = sel.LastDate
	INNER JOIN dbo.IndividualApartmentTypes t ON t.Id = a.IndividualApartmentTypeId
GO
