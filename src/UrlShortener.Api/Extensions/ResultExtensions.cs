using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application.Common.Models;

namespace UrlShortener.Api.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToProblem(this Result result)
    {
        if (result.IsSuccess)
            throw new InvalidOperationException("Cannot convert success result to a problem");


        // إنشاء ProblemDetails بشكل مباشر ونظيف بدون Reflection
        var problemDetails = new ProblemDetails
        {
            Status = result.Error.StatusCode, // بناءً على تعديلك للـ Error ريكورد
            Title = "An error occurred while processing your request.",
            Detail = result.Error.Description
        };

        // إضافة الأخطاء المخصصة في الـ Extensions
        problemDetails.Extensions["errors"] = new[]
        {
            result.Error.Code,
            result.Error.Description
        };

        return new ObjectResult(problemDetails)
        {
            StatusCode = result.Error.StatusCode
        };
    }
}