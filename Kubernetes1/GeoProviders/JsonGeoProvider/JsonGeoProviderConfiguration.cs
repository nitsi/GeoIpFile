using Kubernetes1.Configuration;

namespace Kubernetes1.GeoProviders.JsonGeoProvider
{
    /// <summary>
    /// Configuration for <see cref="JsonGeoProvider"/>.
    /// </summary>
    public class JsonGeoProviderConfiguration : ConfigurationBase
    {
        /// <summary>
        /// The minimum number of minutes to wait before refreshing the data.
        /// </summary>
        public int RefreshIntervalInMinutes { get; set; }

        /// <summary>
        /// Full file path to the JSON file that contains all the geo data.
        /// </summary>
        public string FullFilePath { get; set; }

        /// <summary>
        /// Is the provider enable.
        /// </summary>
        public bool Enabled { get; set; }
    }
}