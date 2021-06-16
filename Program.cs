using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WindsorPricesMonitoring.Code.Classes;
using WindsorPricesMonitoring.Code.Model;
using WindsorPricesMonitoring.Interfaces;

namespace WindsorPricesMonitoring
{
	class Program
	{
		static async Task Main(string[] args)
		{
			var serviceProvider = ConfigureServices();

			var launcher = serviceProvider.GetRequiredService<ILauncher>();

			await launcher.Launch();
		}

		private static IServiceProvider ConfigureServices()
		{
			var services = new ServiceCollection()
				.AddLogging()
				.AddTransient<ILauncher, Launcher>()
				.AddTransient<IHtmlParser, HtmlParser>()
				.AddScoped<IRepository, Repository>();

			services.AddDbContext<WindsorPricesMonitoringDbContext>(options =>
				options.UseSqlServer(@"Server=.\;Database=WindsorMonitoring;User Id=ModernUser;Password=qwerty;"));

			return services.BuildServiceProvider();
		}
	}
}
