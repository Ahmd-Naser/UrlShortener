
namespace UrlShortener.Application.Url.Queries.GetOriginalUrl;

public record GetOriginalUrlQuery(
    string ShortenedUrl
) : IRequest<OriginalUrlResponse>;
