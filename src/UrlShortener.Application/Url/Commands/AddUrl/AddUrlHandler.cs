using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using UrlShortener.Application.Common.Models;
using UrlShortener.Application.Services;

namespace UrlShortener.Application.Url.Commands.AddUrl;

public class AddUrlHandler(IUrlService urlService) : IRequestHandler<AddUrlCommand, Result<ShortenedUrl>>
{
    private readonly IUrlService _urlService = urlService;

    public async Task<Result<ShortenedUrl>> Handle(AddUrlCommand request, CancellationToken cancellationToken = default )
    {
       return await _urlService.AddUrl(request, cancellationToken);
    }
}
