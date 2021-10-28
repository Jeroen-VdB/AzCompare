using AzCompare.Options;
using AzCompare.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace AzCompare.ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).RunConsoleAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<AzComparer>();
                    services.Configure<AzCompareOptions>(hostContext.Configuration.GetSection(nameof(AzCompareOptions)));
                    services.AddScoped<IAzCompareService, AzCompareService>();
                });
    }
}
