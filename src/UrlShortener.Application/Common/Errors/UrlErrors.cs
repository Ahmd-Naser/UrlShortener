using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using UrlShortener.Application.Common.Models;

namespace UrlShortener.Application.Common.Errors;

public static class UrlErrors
{
    public static Error NotFound(string shortenedUrl) =>
        new Error("Url.NotFound", $"the Url :  '{shortenedUrl}'  was not found.", (int)HttpStatusCodes.NotFound);


}