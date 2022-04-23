using System;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Kubernetes1.Throttling
{
    public class ThrottlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ThrottlingMiddlewareConfiguration _configuration;
        private readonly MemoryCache _memoryCache = new (nameof(ThrottlingMiddleware));

        public ThrottlingMiddleware(RequestDelegate next, ThrottlingMiddlewareConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public Task Invoke(HttpContext context)
        {
            if (context.Connection.RemoteIpAddress == null)
            {
                context.Response.StatusCode = 400;
                return Task.FromResult((RequestDelegate)null);
            }

            var newCacheItem = new ThrottlingItem
            {
                Count = 0
            };

            var throttlingItem = _memoryCache.AddOrGetExisting(
                context.Connection.RemoteIpAddress.ToString(),
                newCacheItem,
                DateTimeOffset.UtcNow.AddSeconds(_configuration.WindowSizeInSeconds)
                ) as ThrottlingItem ?? newCacheItem;

            Interlocked.Add(ref throttlingItem.Count, 1);

            if (throttlingItem.Count > _configuration.WindowSizeInSeconds * _configuration.EventsPerSecond)
            {
                context.Response.StatusCode = 429;
                return Task.FromResult((RequestDelegate)null);
            }

            return _next(context);
        }
    }
}