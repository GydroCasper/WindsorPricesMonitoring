using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
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
				.AddTransient<ILauncher, Launcher>()
				.AddTransient<IHtmlParser, HtmlParser>()
				.AddScoped<IRepository, Repository>();

			Configure(services);

			ConfigureLogging(services);

			return services.BuildServiceProvider();
		}

		private static void ConfigureLogging(IServiceCollection services)
		{
			services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));

			Log.Logger = new LoggerConfiguration()
				.Enrich.FromLogContext()
				.MinimumLevel.Information()
				.WriteTo.Console()
				.WriteTo.File(@"C:\log\log.txt", rollingInterval: RollingInterval.Day)
				.CreateLogger();
		}

		private static void Configure(IServiceCollection services)
		{
			var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

			var configuration = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", true, true)
				.AddJsonFile($"appsetings.{environmentName}.json", true, true)
				.Build();

			services.AddDbContext<WindsorPricesMonitoringDbContext>(options =>
				options.UseSqlServer(configuration.GetConnectionString("WindsorPricesMonitoring"), builder =>
				{
					builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
				}));

			services.AddScoped<IConfiguration>(_ => configuration);
		}
	}
}
