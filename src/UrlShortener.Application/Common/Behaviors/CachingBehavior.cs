using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using UrlShortener.Application.Common.Interfaces;

namespace UrlShortener.Application.Common.Behaviors;

public class CachingBehavior<TRequest, TResponse>(
    IDistributedCache cache,
    ILogger<CachingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICacheableQuery
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var cacheKey = request.CacheKey;

        // 1. محاولة قراءة الداتا من Redis كنص
        var cachedData = await cache.GetStringAsync(cacheKey, cancellationToken);

        if (!string.IsNullOrEmpty(cachedData))
        {
            logger.LogInformation("Fetched from Redis Cache -> '{CacheKey}'", cacheKey);
            // تحويل النص لكائن C# تاني
            return JsonSerializer.Deserialize<TResponse>(cachedData)!;
        }

        logger.LogInformation("Redis Cache Miss -> '{CacheKey}'", cacheKey);

        // 2. لو مش موجودة، هنكمل للـ Handler عشان يجيبها من الداتابيز
        var response = await next();

        if (response != null)
        {
            // 3. تحويل الكائن لنص عشان يتخزن في Redis
            var serializedData = JsonSerializer.Serialize(response);

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = request.Expiration ?? TimeSpan.FromHours(1)
            };

            await cache.SetStringAsync(cacheKey, serializedData, options, cancellationToken);
        }

        return response;
    }
}