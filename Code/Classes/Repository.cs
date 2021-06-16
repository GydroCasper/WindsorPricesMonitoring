using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WindsorPricesMonitoring.Code.Dto;
using WindsorPricesMonitoring.Code.Model;
using WindsorPricesMonitoring.Interfaces;
using ApartmentType = WindsorPricesMonitoring.Code.Dto.ApartmentType;

namespace WindsorPricesMonitoring.Code.Classes
{
	public class Repository: IRepository, IDisposable
	{
		private readonly WindsorPricesMonitoringDbContext _db;

		private List<ApartmentType> _apartmentTypes;

		public Repository(WindsorPricesMonitoringDbContext db)
		{
			_db = db;
		}

		private async Task<List<ApartmentType>> GetApartmentTypes()
		{
			return _apartmentTypes ??= await _db.ApartmentTypes.Select(x => new ApartmentType {Id = x.Id, Name = x.Name}).ToListAsync();
		}

		public async Task SaveApartmentsDataForToday(List<Apartment> apartments)
		{
			var lastPrices = await _db.LastApartmentAvailability.ToListAsync();
			var apartmentTypes = await GetApartmentTypes();

			var apartmentsToAdd = new List<ApartmentTypeAvailability>();

			foreach (var apartment in apartments)
			{
				if (!lastPrices.Any(x =>
					x.Type == apartment.Name && 
					x.Available == apartment.Availability &&
					x.MinimumPrice == apartment.Rent))
				{
					var typeId = apartmentTypes.Single(x => x.Name == apartment.Name).Id;

					apartmentsToAdd.Add(new ApartmentTypeAvailability
					{
						Available = apartment.Availability, 
						Date = DateTime.Now, 
						MinimumPrice = apartment.Rent,
						TypeId = typeId
					});
				}
			}

			if (apartmentsToAdd.Any())
			{
				_db.ApartmentTypeAvailability.AddRange(apartmentsToAdd);
				await _db.SaveChangesAsync();
			}
		}

		public void Dispose()
		{
			_db?.Dispose();
		}
	}
}