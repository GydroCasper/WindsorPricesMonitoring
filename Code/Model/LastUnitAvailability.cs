using System;

namespace WindsorPricesMonitoring.Code.Model
{
	public class LastUnitAvailability
	{
		public Guid Id { get; set; }
		public string FullNumber { get; set; }
		public Guid TypeId { get; set; }
		public DateTime Date { get; set; }
		public DateTime? DateAvailable { get; set; }
		public bool IsAvailable { get; set; }
		public short? MinimumPrice { get; set; }
	}
}