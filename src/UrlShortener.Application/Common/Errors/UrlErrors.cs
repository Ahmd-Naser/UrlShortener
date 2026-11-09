using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using UrlShortener.Application.Common.Models;

namespace UrlShortener.Application.Common.Errors;

public static class UrlErrors
{
    public static Error NotFound(string shortendUrl) =>
        new Error("Url.NotFound", $"the Url :  \"{shortendUrl}\"  was not found.", (int)HttpStatusCodes.NotFound);


}