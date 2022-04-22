using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Kubernetes1.Infra;
using Kubernetes1.Model;
using Microsoft.Extensions.Logging;

namespace Kubernetes1.GeoProviders.JsonGeoProvider
{
    /// <summary>
    /// Provides data on geo location information from a JSON file.
    /// </summary>
    public class JsonGeoDataProvider
    {
        private readonly ILogger<JsonGeoDataProvider> _logger;
        private readonly JsonGeoProviderConfiguration _configuration;

        public JsonGeoDataProvider(ILogger<JsonGeoDataProvider> logger, JsonGeoProviderConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// Read all JSON file data to memory and return an easy to use hash file.
        /// </summary>
        /// <returns>Easy to use hash of <see cref="IPAddress"/> and <see cref="GeoResult"/></returns>
        public async Task<Dictionary<IPAddress, GeoResult>> ReadDataAsync()
        {
            if (string.IsNullOrWhiteSpace(_configuration.FullFilePath))
            {
                string internalErrorMessage = $"{nameof(JsonGeoProvider)} is missing required configuration value {nameof(JsonGeoProviderConfiguration.FullFilePath)}.";
                _logger.LogError(internalErrorMessage);
                var inner = new ArgumentException(internalErrorMessage, nameof(_configuration));
                throw new HttpException(internalErrorMessage, HttpStatusCode.InternalServerError, WebApiExceptionFilter.InternalServerError, inner);
            }

            if (!File.Exists(_configuration.FullFilePath))
            {
                string internalErrorMessage = $"File provided in {nameof(JsonGeoProviderConfiguration)} dose not exists.";
                _logger.LogError(internalErrorMessage);
                var inner = new ArgumentException(internalErrorMessage, nameof(_configuration));
                throw new HttpException(internalErrorMessage, HttpStatusCode.InternalServerError, WebApiExceptionFilter.InternalServerError, inner);
            }

            try
            {
                var jsonText = await File.ReadAllTextAsync(_configuration.FullFilePath);
                var geoItems = Newtonsoft.Json.JsonConvert.DeserializeObject<JsonGeoItem[]>(jsonText);
                return geoItems?.ToDictionary(keySelector: item => item.Ip, item => new GeoResult(item.Country, item.City));
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Exception while processing data file in {nameof(JsonGeoProvider)} constructor.");
                throw new HttpException(exception.Message, HttpStatusCode.InternalServerError, WebApiExceptionFilter.InternalServerError, exception);
            }
        }
    }
}