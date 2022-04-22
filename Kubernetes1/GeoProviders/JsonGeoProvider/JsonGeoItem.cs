using System.Net;
using Kubernetes1.Infra;
using Newtonsoft.Json;

namespace Kubernetes1.GeoProviders.JsonGeoProvider
{
    /// <summary>
    /// A JSON item in the file.
    /// </summary>
    public class JsonGeoItem
    {
        /// <summary>
        /// The IP Address
        /// </summary>
        [JsonConverter(typeof(IPJsonConverter))]
        public IPAddress Ip { get; set; }

        /// <summary>
        /// The city the <see cref="Ip"/> is in.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// The country the <see cref="Ip"/> is in.
        /// </summary>
        public string Country { get; set; }
    }
}