using System;
using System.Collections.Generic;
using System.Text;

namespace UrlShortener.Application.Url.Events;

public record UrlVisitedEvent(
    string ShortenedCode,
    string? IpAddress,
    string? UserAgent
) : INotification;