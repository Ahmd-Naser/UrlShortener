using System;
using System.Collections.Generic;
using System.Text;
using UrlShortener.Application.Common.Models;
using UrlShortener.Application.Services;

namespace UrlShortener.Application.Url.Queries.GetOriginalUrl;

public class GetOriginalUrlQueryHandler(IUrlService urlService) : IRequestHandler<GetOriginalUrlQuery, Result<OriginalUrlResponse>>
{
    private readonly IUrlService _urlService = urlService;

    public Task<Result<OriginalUrlResponse>> Handle(GetOriginalUrlQuery request, CancellationToken cancellationToken)
    {
        return _urlService.GetOriginalUrl(request, cancellationToken);
    }
}
