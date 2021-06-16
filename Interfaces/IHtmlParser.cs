using System.Collections.Generic;
using WindsorPricesMonitoring.Code.Dto;

namespace WindsorPricesMonitoring.Interfaces
{
	public interface IHtmlParser
	{
		(IEnumerable<Apartment>, IEnumerable<Unit>) GetAndParse();
	}
}