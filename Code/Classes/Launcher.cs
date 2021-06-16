using System.Linq;
using System.Threading.Tasks;
using WindsorPricesMonitoring.Interfaces;

namespace WindsorPricesMonitoring.Code.Classes
{
	public class Launcher: ILauncher
	{
		private readonly IHtmlParser _htmlParser;
		private readonly IRepository _repository;

		public Launcher(IHtmlParser htmlParser, IRepository repository)
		{
			_htmlParser = htmlParser;
			_repository = repository;
		}

		public async Task Launch()
		{
			var apartments = _htmlParser.GetAndParse().ToList();

			await _repository.SaveApartmentsDataForToday(apartments);
		}
	}
}