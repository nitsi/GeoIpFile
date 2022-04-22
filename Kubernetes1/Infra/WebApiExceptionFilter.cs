using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Kubernetes1.Infra
{
    /// <summary>
    /// This class handles all unhandled exception thrown from the code and return a nice customer facing message.
    /// </summary>
    public class WebApiExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<WebApiExceptionFilter> _logger;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly WebApiExceptionFilterConfiguration _configuration;
        public const string InternalServerError = "Internal server error";


        public WebApiExceptionFilter(ILogger<WebApiExceptionFilter> logger, IHostEnvironment hostEnvironment, WebApiExceptionFilterConfiguration configuration)
        {
            _logger = logger;
            _hostEnvironment = hostEnvironment;
            _configuration = configuration;
        }

        /// <summary>
        /// Handles exception to return customer friendly message.
        /// If on development env, return the original exception.
        /// </summary>
        /// <param name="context"></param>
        public void OnException(ExceptionContext context)
        {
            if (_hostEnvironment.IsDevelopment() && _configuration.ShowInternalExceptionForDevelopment)
            {
                context.Result = new ContentResult
                {
                    Content = context.Exception.ToString()
                };

                // Display exception details only when running in Development.
                return;
            }

            if (context.Exception is HttpException contextException)
            {
                context.Result = new JsonResult(new ErrorResponse { Error = contextException.CustomerFriendlyMessage })
                {
                    StatusCode = (int)contextException.HttpStatusCode,
                    ContentType = "application/json"
                };

                return;
            }

            // Default response on an unhandled error
            _logger.LogError(context.Exception, "There was an unhandled exception thrown from the service.");
            context.Result = new JsonResult(new ErrorResponse { Error = InternalServerError })
            {
                StatusCode = 500,
                ContentType = "application/json"
            };
        }
    }
}