using MediatR;

namespace UrlShortener.Application.Url.Commands.AddUrl;

public record AddUrlCommand(
    string OriginalUrl
) : IRequest<ShortenedUrl>;