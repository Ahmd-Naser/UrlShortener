using MediatR;
using UrlShortener.Application.Common.Models;

namespace UrlShortener.Application.Url.Commands.AddUrl;

public record AddUrlCommand(
    string OriginalUrl
) : IRequest<Result<ShortenedUrl>>;