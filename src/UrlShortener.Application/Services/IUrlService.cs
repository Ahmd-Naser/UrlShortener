using System;
using System.Collections.Generic;
using System.Text;
using UrlShortener.Application.Common.Models;
using UrlShortener.Application.Url.Commands.AddUrl;
using UrlShortener.Application.Url.Dtos;
using UrlShortener.Application.Url.Queries.GetOriginalUrl;

namespace UrlShortener.Application.Services;

public interface IUrlService
{
    Task<Result<ShortenedUrl>> AddUrl(AddUrlCommand request, CancellationToken cancellationToken);

    Task<Result<OriginalUrlResponse>> GetOriginalUrl(GetOriginalUrlQuery request, CancellationToken cancellationToken);

    Task RecordVisitAsync(VisitDetailsDto visitDetails, CancellationToken cancellationToken);
}
