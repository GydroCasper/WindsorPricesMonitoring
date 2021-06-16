using System;

namespace WindsorPricesMonitoring.Code.Model
{
	public class IndividualApartmentType
	{
		public Guid Id { get; set; }
		public string FullNumber { get; set; }
		public Guid TypeId { get; set; }
		public short Sqft { get; set; }
	}
}