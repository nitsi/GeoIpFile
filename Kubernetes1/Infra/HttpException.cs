using System;
using System.Net;

namespace Kubernetes1.Infra
{
    /// <summary>
    /// Use this class to throw exceptions that will reach the customer.
    /// </summary>
    public class HttpException : Exception
    {
        /// <summary>
        /// The status code to return to the customer.
        /// </summary>
        public HttpStatusCode HttpStatusCode { get; }

        /// <summary>
        /// An error message to show the customer.
        /// </summary>
        public string CustomerFriendlyMessage { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="HttpException"/>.
        /// </summary>
        /// <param name="internalMessage">Used for internal developer use (same as all other exceptions).</param>
        /// <param name="httpStatusCode">The status code to return to the customer.</param>
        /// <param name="customerFriendlyMessage">An error message to show the customer.</param>
        /// <param name="innerException">Inner exception.</param>
        public HttpException(string internalMessage, HttpStatusCode httpStatusCode, string customerFriendlyMessage, Exception innerException = null) : base(internalMessage, innerException)
        {
            HttpStatusCode = httpStatusCode;
            CustomerFriendlyMessage = customerFriendlyMessage;
        }
    }
}