using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Kubernetes1.Model;

namespace Kubernetes1.GeoProviders
{
    /// <summary>
    /// Interface to extend when creating new geo providers.
    /// </summary>
    public interface IGeoProvider
    {
        /// <summary>
        /// Retrieve the goe information requested.
        /// </summary>
        /// <param name="ip">The IP Address we wish to get the information on.</param>
        /// <returns><see cref="GeoResult"/> when found; otherwise default value.</returns>
        public Task<GeoResult> GetGeoInformationByIpAsync(IPAddress ip, CancellationToken cancellationToken);

        /// <summary>
        /// Gets if the configuration is enabled.
        /// </summary>
        public bool IsEnabled { get; }
    }
}