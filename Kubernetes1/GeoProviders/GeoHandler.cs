using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Kubernetes1.Model;
using Microsoft.Extensions.Logging;

namespace Kubernetes1.GeoProviders
{
    /// <summary>
    /// Handles GEO requests from information.
    /// </summary>
    public class GeoHandler
    {
        private readonly GeoProvidersManagement _geoProvidersManagement;

        public GeoHandler(GeoProvidersManagement geoProvidersManagement)
        {
            _geoProvidersManagement = geoProvidersManagement;
        }

        /// <summary>
        /// Get the first value from any of the geo providers, runs in parallel.
        /// </summary>
        /// <param name="ipAddress"><see cref="IPAddress"/> to lookup in all providers.</param>
        /// <returns></returns>
        public async Task<GeoResult> GetFirstOrDefaultGeoInformationByIpAsync(IPAddress ipAddress, CancellationToken cancellationToken)
        {
            var tasks = new List<Task<GeoResult>>();
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            var providers = _geoProvidersManagement.GetActiveProviders();
            foreach (var geoProvider in providers)
            {
                tasks.Add(geoProvider.GetGeoInformationByIpAsync(ipAddress, linkedTokenSource.Token));
            }

            await Task.WhenAny(tasks);
            bool allTasksCompleted = false;
            while (!allTasksCompleted)
            {
                allTasksCompleted = true;
                foreach (var task in tasks)
                {
                    if (task.Status is
                        TaskStatus.Created or
                        TaskStatus.Running or
                        TaskStatus.WaitingForActivation or
                        TaskStatus.WaitingForChildrenToComplete or
                        TaskStatus.WaitingToRun)
                    {
                        allTasksCompleted = false;
                    }

                    if (task.IsCompletedSuccessfully && task.Result != null)
                    {
                        linkedTokenSource.Cancel(false);
                        return task.Result;
                    }
                }
            }

            return default;
        }
    }
}