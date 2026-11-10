
using UrlShortener.Application.Common.Interfaces;
using UrlShortener.Application.Common.Models;

namespace UrlShortener.Application.Url.Queries.GetOriginalUrl;

public record GetOriginalUrlQuery(
    string ShortenedUrl
) : IRequest<Result<OriginalUrlResponse>>, ICacheableQuery
{
    public string CacheKey => $"url_{ShortenedUrl}";

    // هنخلي الرابط يعيش في الكاش لمدة يوم كامل طالما عليه زيارات
    public TimeSpan? Expiration => TimeSpan.FromDays(1);
}
