using DiffPlex.DiffBuilder.Model;
using System.Threading;
using System.Threading.Tasks;

namespace AzCompare.Services
{
    public interface IAzCompareService
    {
        public Task<DiffPaneModel> CompareAsync(CancellationToken cancellationToken);
    }
}
