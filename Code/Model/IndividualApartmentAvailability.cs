using System;

namespace WindsorPricesMonitoring.Code.Model
{
	public class IndividualApartmentAvailability
	{
		public Guid Id { get; set; }
		public Guid IndividualApartmentTypeId { get; set; }
		public DateTime Date { get; set; }
		public DateTime? DateAvailable { get; set; }
		public bool IsAvailable { get; set; }
		public short? MinimumPrice { get; set; }
	}
}