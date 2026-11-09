
using UrlShortener.Application.Common.Models;

namespace UrlShortener.Application.Url.Queries.GetOriginalUrl;

public record GetOriginalUrlQuery(
    string ShortenedUrl
) : IRequest<Result<OriginalUrlResponse>>;
