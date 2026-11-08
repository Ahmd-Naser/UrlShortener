using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using UrlShortener.Application.Services;

namespace UrlShortener.Application.Url.Commands.AddUrl;

public class AddUrlHandler(IUrlService urlService) : IRequestHandler<AddUrlCommand, ShortenedUrl>
{
    private readonly IUrlService _urlService = urlService;

    public Task<ShortenedUrl> Handle(AddUrlCommand request, CancellationToken cancellationToken = default )
    {
       return _urlService.AddUrl(request, cancellationToken);
    }
}
