using System;
using System.Collections.Generic;
using System.Text;
using UrlShortener.Application.Url.Commands.AddUrl;

namespace UrlShortener.Application.Services;

public interface IUrlService
{
    Task<ShortenedUrl> AddUrl(AddUrlCommand request, CancellationToken cancellationToken);
}
