using System.Collections.Generic;
using System.Threading.Tasks;
using WindsorPricesMonitoring.Code.Dto;

namespace WindsorPricesMonitoring.Interfaces
{
	public interface IRepository
	{
		Task SaveApartmentsDataForToday(List<Apartment> apartments);
	}
}