using System;

namespace WindsorPricesMonitoring.Code.Model
{
	public class LastApartmentAvailability
	{
		public Guid Id { get; set; }
		public string Type { get; set; }
		public Guid TypeId { get; set; }
		public DateTime Date { get; set; }
		public byte Available { get; set; }
		public short? MinimumPrice { get; set; }

	}
}