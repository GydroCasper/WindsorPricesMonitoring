using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WindsorPricesMonitoring.Interfaces;

namespace WindsorPricesMonitoring.Code.Classes
{
	public class Launcher: ILauncher
	{
		private readonly IHtmlParser _htmlParser;
		private readonly IRepository _repository;
		private readonly ILogger<Launcher> _logger;

		public Launcher(IHtmlParser htmlParser, IRepository repository, ILogger<Launcher> logger)
		{
			_htmlParser = htmlParser;
			_repository = repository;
			_logger = logger;
		}

		public async Task Launch()
		{
			try
			{
				_logger.LogInformation("The program is started");
				var (apartments, units) = _htmlParser.GetAndParse();

				await _repository.SaveDataForToday(apartments, units.ToList());

				_logger.LogInformation("The program is finished successfully");
			}
			catch (Exception e)
			{
				_logger.LogInformation($"The program is failed. Exception: {e.Message}. StackTrace: {e.StackTrace}");
				throw;
			}
		}
	}
}