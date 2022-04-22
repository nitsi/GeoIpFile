using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Kubernetes1.GeoProviders;
using Kubernetes1.Infra;
using Kubernetes1.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Kubernetes1.Controllers
{
    [ApiController]
    [Route("v1")]
    [TypeFilter(typeof(WebApiExceptionFilter))]
    public class GeoController : ControllerBase
    {
        private readonly GeoHandler _geoHandler;

        /// <summary>
        /// Initialize a new instance of <see cref="GeoController"/>.
        /// </summary>
        /// <param name="geoHandler">Handles GEO requests from information.</param>
        public GeoController(GeoHandler geoHandler)
        {
            _geoHandler = geoHandler;
        }

        [HttpGet]
        [Route("find-country")]
        public async Task<IActionResult> GetCountryByIpAsync(string ip, CancellationToken cancellationToken)
        {
            if (!IPAddress.TryParse(ip, out var ipAddress))
            {
                var errorMessage = "IP Address was not in a valid format";
                throw new HttpException(errorMessage, HttpStatusCode.BadRequest, errorMessage);
            }

            var result = await _geoHandler.GetFirstOrDefaultGeoInformationByIpAsync(ipAddress, cancellationToken);

            if (result != default)
            {
                return Ok(result);
            }

            return NotFound(new ErrorResponse { Error = "IP was not found in our databases." });
        }
    }
}