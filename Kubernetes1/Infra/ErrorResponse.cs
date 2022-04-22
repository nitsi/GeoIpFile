using Newtonsoft.Json;

namespace Kubernetes1.Infra
{
    /// <summary>
    /// Use this class to serialize an error we want to return.
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// Customer facing error.
        /// </summary>
        [JsonProperty(PropertyName = "error")]
        public string Error { get; set; }
    }
}