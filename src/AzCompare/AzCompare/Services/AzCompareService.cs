using AzCompare.Options;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Resources.Models;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AzCompare.Services
{
    public class AzCompareService : IAzCompareService
    {
        private readonly ILogger<AzCompareService> _logger;
        private readonly IOptions<AzCompareOptions> _azCompareOptions;
        private readonly ArmClient _armClient;

        public AzCompareService(ILogger<AzCompareService> logger, IOptions<AzCompareOptions> azCompareOptions)
        {
            _logger = logger;
            _azCompareOptions = azCompareOptions;
            _armClient = new ArmClient(new DefaultAzureCredential());
        }

        public async Task<DiffPaneModel> CompareAsync(CancellationToken cancellationToken)
        {
            string leftResources;
            string rightResources;

            if (_azCompareOptions.Value.Scope?.ToLower().Equals("resourcegroup") ?? false)
            {
                leftResources = await GetResourcesFromRgAsync(_azCompareOptions.Value.LeftId, cancellationToken);
                rightResources = await GetResourcesFromRgAsync(_azCompareOptions.Value.RightId, cancellationToken);
            }
            else
            {
                leftResources = await GetResourcesFromSubscriptionAsync(_azCompareOptions.Value.LeftId, cancellationToken);
                rightResources = await GetResourcesFromSubscriptionAsync(_azCompareOptions.Value.RightId, cancellationToken);
            }

            var diff = InlineDiffBuilder.Diff(
                ApplyFilter(leftResources, _azCompareOptions.Value.LeftFilter), 
                ApplyFilter(rightResources, _azCompareOptions.Value.RightFilter));

            _logger.LogInformation("Finished comparison");

            return diff;
        }

        private string ApplyFilter(string arm, string filter) => string.IsNullOrEmpty(filter)
            ? arm 
            : arm.Replace(filter, string.Empty);

        private async Task<string> GetResourcesFromSubscriptionAsync(string id, CancellationToken cancellationToken)
        {
            var result = new List<ResourceGroupExportResult>();

            var subscription = _armClient.GetSubscription(new ResourceIdentifier($"/subscriptions/{id}"));
            _logger.LogInformation("Subscription: {subscription}", id);

            var rgs = subscription.GetResourceGroups().GetAllAsync(null, null, cancellationToken);

            await foreach (var rg in rgs)
            {
                _logger.LogInformation("ResourceGroup: {resourceGroup}", rg.Data.Id + ": " + rg.Data.Name);
                result.Add(await GetArmTemplate(rg, cancellationToken));
            }

            return ArmToJson(result);
        }

        private async Task<string> GetResourcesFromRgAsync(string id, CancellationToken cancellationToken)
        {
            var rg = _armClient.GetResourceGroup(new ResourceIdentifier(id));
            _logger.LogInformation("ResourceGroup: {resourceGroup}", id);

            return ArmToJson(await GetArmTemplate(rg, cancellationToken));
        }

        private async Task<ResourceGroupExportResult> GetArmTemplate(ResourceGroup rg, CancellationToken cancellationToken)
        {
            var request = new ExportTemplateRequest();
            request.Options = "SkipAllParameterization";
            request.Resources.Add("*");
            var armtemplate = await rg.ExportTemplateAsync(request, true, cancellationToken);

            return armtemplate.Value;
        }

        private string ArmToJson(object arm) 
            => JsonSerializer.Serialize(arm, new JsonSerializerOptions { WriteIndented = true });
    }
}
