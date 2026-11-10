using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using UrlShortener.Application.Common.Interfaces;

namespace UrlShortener.Application.Common.Behaviors;

public class CachingBehavior<TRequest, TResponse>(
    IMemoryCache cache,
    ILogger<CachingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // لو الـ Request الحالي مش بيطبق الانترفيس، كمل الـ Pipeline عادي جداً بدون كاش
        if (request is not ICacheableQuery cacheableQuery)
        {
            return await next();
        }

        // لو الداتا موجودة في الكاش، رجعها علطول (كده وفرنا رحلة للداتابيز!)
        if (cache.TryGetValue(cacheableQuery.CacheKey, out TResponse? cachedResponse))
        {
            logger.LogInformation("Fetched from Cache -> '{CacheKey}'", cacheableQuery.CacheKey);
            return cachedResponse!;
        }

        // لو مش موجودة، هنكمل الـ Pipeline عشان نجيبها من الداتابيز
        var response = await next();

        // هنحدد وقت الكاش (ساعة افتراضياً لو مش متحدد)
        var slidingExpiration = cacheableQuery.Expiration ?? TimeSpan.FromHours(1);
        var cacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(slidingExpiration);

        // نحفظها في الكاش للمرات الجاية
        cache.Set(cacheableQuery.CacheKey, response, cacheOptions);
        logger.LogInformation("Added to Cache -> '{CacheKey}'", cacheableQuery.CacheKey);

        return response;
    }
}