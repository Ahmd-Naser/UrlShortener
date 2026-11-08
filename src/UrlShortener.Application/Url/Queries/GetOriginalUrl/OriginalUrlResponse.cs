using System;
using System.Collections.Generic;
using System.Text;

namespace UrlShortener.Application.Url.Queries.GetOriginalUrl;

public record OriginalUrlResponse(
    string Url
);