using System.Collections.Generic;
using System.Threading.Tasks;
using WindsorPricesMonitoring.Code.Dto;

namespace WindsorPricesMonitoring.Interfaces
{
	public interface IRepository
	{
		Task SaveDataForToday(IEnumerable<Apartment> apartments, IList<Unit> units);
	}
}