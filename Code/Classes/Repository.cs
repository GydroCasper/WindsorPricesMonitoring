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
		private List<IndividualApartmentType> _unitTypes;

		public Repository(WindsorPricesMonitoringDbContext db)
		{
			_db = db;
		}

		private async Task<List<ApartmentType>> GetApartmentTypes()
		{
			return _apartmentTypes ??= await _db.ApartmentTypes.Select(x => new ApartmentType {Id = x.Id, Name = x.Name}).ToListAsync();
		}

		private async Task<List<IndividualApartmentType>> GetUnitTypes()
		{
			return _unitTypes ??= await _db.IndividualApartmentTypes
				.Select(x => new IndividualApartmentType {Id = x.Id, FullNumber = x.FullNumber}).ToListAsync();
		}

		public async Task SaveDataForToday(IEnumerable<Apartment> apartments, IList<Unit> units)
		{
			await SaveApartmentsDataForToday(apartments);
			await SaveUnitsDataForToday(units);
		}

		private async Task SaveUnitsDataForToday(IList<Unit> units)
		{
			var lastPrices = await _db.LastUnitAvailability.ToListAsync();
			var unitTypes = await GetUnitTypes();

			var unitsToAdd = new List<IndividualApartmentAvailability>();
			var unitTypesToAdd = new List<IndividualApartmentType>();

			foreach (var unit in units)
			{
				if (IfUnitDataChanged(lastPrices, unit) ||
				    IfUnavailableUnitBecameAvailable(lastPrices, unit.FullNumber))
				{
					var typeId = unitTypes.SingleOrDefault(x => x.FullNumber == unit.FullNumber)?.Id;

					if (!typeId.HasValue)
					{
						typeId = Guid.NewGuid();
						var apartmentTypes = await GetApartmentTypes();
						unitTypesToAdd.Add(new IndividualApartmentType
						{
							Id = typeId.Value,
							FullNumber = unit.FullNumber,
							Sqft = unit.Area,
							TypeId = apartmentTypes.Single(x => x.Name == unit.UnitType).Id
						});
					}

					if (!unit.DateAvailable.HasValue)
					{
						var lastAvailableDate = _db.IndividualApartmentAvailability.Where(x =>
								x.IndividualApartmentTypeId == typeId && x.IsAvailable && x.DateAvailable != null &&
								!_db.IndividualApartmentAvailability.Any(y =>
									!y.IsAvailable &&
									y.Date > x.Date &&
									y.IndividualApartmentTypeId == x.IndividualApartmentTypeId))
							.OrderByDescending(x => x.DateAvailable).FirstOrDefault();

						if (lastAvailableDate?.DateAvailable != null)
						{
							unit.DateAvailable = lastAvailableDate.DateAvailable.Value;
						}
					}

					unitsToAdd.Add(new IndividualApartmentAvailability
					{
						IndividualApartmentTypeId = typeId.Value,
						MinimumPrice = unit.MinimumPrice,
						DateAvailable = unit.DateAvailable,
						IsAvailable = true,
						Date = DateTime.Now
					});
				}
			}

			unitsToAdd.AddRange(lastPrices.Where(x => x.IsAvailable && units.All(y => y.FullNumber != x.FullNumber))
				.Select(x => new IndividualApartmentAvailability
					{IndividualApartmentTypeId = x.TypeId, Date = DateTime.Now, IsAvailable = false}));

			if (unitTypesToAdd.Any())
			{
				_db.IndividualApartmentTypes.AddRange(unitTypesToAdd);
				await _db.SaveChangesAsync();
			}

			if (unitsToAdd.Any())
			{
				var todayUnits = _db.IndividualApartmentAvailability.Where(x => x.Date == DateTime.Today);
				_db.IndividualApartmentAvailability.RemoveRange(todayUnits);

				_db.IndividualApartmentAvailability.AddRange(unitsToAdd);
				await _db.SaveChangesAsync();
			}
		}

		private static bool IfUnavailableUnitBecameAvailable(IEnumerable<LastUnitAvailability> lastPrices, string fullNumber)
		{
			return lastPrices.Any(x => x.FullNumber == fullNumber && !x.IsAvailable);
		}

		private static bool IfUnitDataChanged(IEnumerable<LastUnitAvailability> lastPrices, Unit unit)
		{
			return !lastPrices.Any(x =>
				x.FullNumber == unit.FullNumber &&
				x.DateAvailable == unit.DateAvailable &&
				x.MinimumPrice == unit.MinimumPrice &&
				x.IsAvailable);
		}

		private async Task SaveApartmentsDataForToday(IEnumerable<Apartment> apartments)
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