using System;

namespace WindsorPricesMonitoring.Code.Model
{
	public class ApartmentTypeAvailability
	{
		public Guid Id { get; set; }
		public DateTime Date { get; set; }
		public byte Available { get; set; }
		public Guid TypeId { get; set; }
		public short? MinimumPrice { get; set; }
	}
}