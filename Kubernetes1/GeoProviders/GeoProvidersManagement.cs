using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Kubernetes1.GeoProviders
{
    /// <summary>
    /// Manage a thread safe array of <see cref="IGeoProvider"/>s.
    /// </summary>
    public class GeoProvidersManagement
    {
        private readonly ILogger<GeoProvidersManagement> _logger;
        private readonly List<IGeoProvider> _geoProviders;
        private readonly ReaderWriterLock _lock = new();

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="logger">logger.</param>
        /// <param name="geoProviders">DI geo providers.</param>
        public GeoProvidersManagement(ILogger<GeoProvidersManagement> logger, IGeoProvider[] geoProviders)
        {
            _logger = logger;
            _geoProviders = geoProviders.ToList();
        }

        /// <summary>
        /// Return all providers currently registered and enabled.
        /// </summary>
        /// <returns></returns>
        public ReadOnlyCollection<IGeoProvider> GetActiveProviders()
        {
            try
            {
                _lock.AcquireReaderLock(Timeout.Infinite);
                return _geoProviders.Where(p => p.IsEnabled).ToList().AsReadOnly();
            }
            finally
            {
                _lock.ReleaseReaderLock();
            }
        }

        /// <summary>
        /// Register new provider during runtime.
        /// </summary>
        public void RegisterProvider(IGeoProvider provider)
        {
            try
            {
                _logger.LogInformation($"Registering new geo provider - {provider.GetType().Name}");
                _lock.AcquireWriterLock(Timeout.Infinite);
                _geoProviders.Add(provider);
            }
            finally
            {
                _lock.ReleaseWriterLock();
            }
        }

        /// <summary>
        /// Try to remove provider during runtime
        /// </summary>
        /// <returns>True if provider was removed.</returns>
        public bool TryRemoveProvider(IGeoProvider provider)
        {
            try
            {
                _logger.LogInformation($"Removing geo provider - {provider.GetType().Name}");
                _lock.AcquireWriterLock(Timeout.Infinite);
                return _geoProviders.Remove(provider);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error wile removing provider.");
            }
            finally
            {
                _lock.ReleaseWriterLock();
            }

            return false;
        }
    }
}