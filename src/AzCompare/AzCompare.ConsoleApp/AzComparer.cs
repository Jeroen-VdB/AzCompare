using AzCompare.Options;
using AzCompare.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AzCompare.ConsoleApp
{
    public class AzComparer : IHostedService
    {
        private readonly ILogger<AzComparer> _logger;
        private readonly IHostApplicationLifetime _lifetime;
        private readonly IAzCompareService _azCompareService;
        private readonly IOptions<AzCompareOptions> _azCompareOptions;

        public AzComparer(ILogger<AzComparer> logger, IHostApplicationLifetime lifetime, IAzCompareService azCompareService, IOptions<AzCompareOptions> azCompareOptions)
        {
            _logger = logger;
            _lifetime = lifetime;
            _azCompareService = azCompareService;
            _azCompareOptions = azCompareOptions;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Starting AzComparer");

                if (string.IsNullOrEmpty(_azCompareOptions.Value.LeftId) || string.IsNullOrEmpty(_azCompareOptions.Value.RightId))
                {
                    _logger.LogError("Missing AzCompareOptions LeftId or RightId in appsettings.json");
                } 
                else
                {
                    var diff = await _azCompareService.CompareAsync(cancellationToken);
                    AzCompareWriter.WriteDiff(diff);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AzComparer encountered an error");
            }
            finally
            {
                _lifetime.StopApplication();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("AzComparer finished");
            return Task.CompletedTask;
        }
    }
}
