using System;
using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Kubernetes1.Throttling
{
    public class ThrottlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ThrottlingMiddlewareConfiguration _configuration;
        private readonly ILogger<ThrottlingMiddleware> _logger;
        private readonly ConcurrentDictionary<IPAddress, ThrottlingItem> _dictionary;

        public ThrottlingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, ThrottlingMiddlewareConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
            _logger = loggerFactory.CreateLogger<ThrottlingMiddleware>();
            _dictionary = new ConcurrentDictionary<IPAddress, ThrottlingItem>();
        }

        public Task Invoke(HttpContext context)
        {
            if (context.Connection.RemoteIpAddress == null)
            {
                context.Response.StatusCode = 404;
                return Task.FromResult((RequestDelegate)null);
            }

            var throttlingItem = _dictionary.AddOrUpdate(context.Connection.RemoteIpAddress,
                address =>
                    new ThrottlingItem
                    {
                        Count = 1,
                        WindowDateTime = DateTime.UtcNow
                    },
                (address, item) =>
                {
                    if (DateTime.UtcNow > item.WindowDateTime.AddSeconds(_configuration.WindowSizeInSeconds))
                    {
                        item.WindowDateTime = DateTime.UtcNow;
                        item.Count = 1;
                    }
                    else
                    {
                        Interlocked.Add(ref item.Count, 1);
                    }

                    return item;
                });

            if (throttlingItem.Count > _configuration.WindowSizeInSeconds * _configuration.EventsPerSecond)
            {
                context.Response.StatusCode = 429;
                return Task.FromResult((RequestDelegate)null);
            }

            return _next(context);
        }
    }
}