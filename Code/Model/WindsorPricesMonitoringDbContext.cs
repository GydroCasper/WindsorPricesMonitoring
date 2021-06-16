using Microsoft.EntityFrameworkCore;

namespace WindsorPricesMonitoring.Code.Model
{
	public class WindsorPricesMonitoringDbContext: DbContext
	{
		public WindsorPricesMonitoringDbContext(DbContextOptions<WindsorPricesMonitoringDbContext> options) : base(options)	{ }

		public DbSet<ApartmentType> ApartmentTypes { get; set; }

		public DbSet<ApartmentTypeAvailability> ApartmentTypeAvailability { get; set; }

		public DbSet<LastApartmentAvailability> LastApartmentAvailability { get; set; }

		public DbSet<LastUnitAvailability> LastUnitAvailability { get; set; }

		public DbSet<IndividualApartmentAvailability> IndividualApartmentAvailability { get; set; }

		public DbSet<IndividualApartmentType> IndividualApartmentTypes { get; set; }
	}
}