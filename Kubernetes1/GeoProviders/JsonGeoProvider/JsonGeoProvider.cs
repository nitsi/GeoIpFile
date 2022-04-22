using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Kubernetes1.Model;
using Microsoft.Extensions.Logging;

namespace Kubernetes1.GeoProviders.JsonGeoProvider
{
    /// <summary>
    /// An <see cref="IGeoProvider"/> that have JSON as the source file.
    /// This is a very memory heavy solution, consider using DB for production.
    /// </summary>
    public class JsonGeoProvider : IGeoProvider, IDisposable
    {
        private readonly ILogger<JsonGeoProvider> _logger;
        private readonly JsonGeoDataProvider _dataProvider;
        private Dictionary<IPAddress, GeoResult> _mapping;
        private readonly Timer _timer;

        public JsonGeoProvider(ILogger<JsonGeoProvider> logger, JsonGeoDataProvider dataProvider, JsonGeoProviderConfiguration configuration)
        {
            _logger = logger;
            _dataProvider = dataProvider;
            
            if (!configuration.Enabled)
            {
                IsEnabled = false;
                return;
            }

            _mapping = dataProvider.ReadDataAsync().Result;
            _timer = new Timer(RefreshCacheAsync);
            _timer.Change(
                TimeSpan.FromMinutes(configuration.RefreshIntervalInMinutes),
                TimeSpan.FromMinutes(configuration.RefreshIntervalInMinutes));

            IsEnabled = true;
        }

        /// <summary>
        /// Retrieve the goe information requested.
        /// </summary>
        /// <param name="ip">The IP Address we wish to get the information on.</param>
        /// <returns><see cref="GeoResult"/> when found; otherwise default value.</returns>
        public Task<GeoResult> GetGeoInformationByIpAsync(IPAddress ip, CancellationToken cancellationToken)
        {
            if (_mapping.TryGetValue(ip, out var item))
            {
                return Task.FromResult(item);
            }

            return Task.FromResult(default(GeoResult));
        }

        /// <summary>
        /// Gets if the configuration is enabled.
        /// </summary>
        public bool IsEnabled { get; private set; }

        private async void RefreshCacheAsync(object? state)
        {
            _logger.LogInformation("Running data refresh.");
            var newMapping = await _dataProvider.ReadDataAsync();
            _mapping = newMapping;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}