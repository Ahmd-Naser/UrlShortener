using System;
using System.Collections.Generic;
using System.Text;

namespace UrlShortener.Application.Url.Dtos;

public record VisitDetailsDto(
    string ShortCode,
    string? IpAddress,
    string? UserAgent
);