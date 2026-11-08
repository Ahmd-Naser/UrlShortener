using System;
using System.Collections.Generic;
using System.Text;
using UrlShortener.Application.Url.Commands.AddUrl;
using UrlShortener.Application.Url.Queries.GetOriginalUrl;

namespace UrlShortener.Application.Services;

public interface IUrlService
{
    Task<ShortenedUrl> AddUrl(AddUrlCommand request, CancellationToken cancellationToken);

    Task<OriginalUrlResponse> GetOriginalUrl(GetOriginalUrlQuery request, CancellationToken cancellationToken);
}
