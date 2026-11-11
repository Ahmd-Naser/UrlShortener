using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using UrlShortener.Api.Extensions;
using UrlShortener.Application.Url.Commands.AddUrl;
using UrlShortener.Application.Url.Events;
using UrlShortener.Application.Url.Queries.GetOriginalUrl;

namespace UrlShortener.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[EnableRateLimiting("GlobalPolicy")]
public class UrlsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost]
    [EnableRateLimiting("CreateUrlPolicy")]
    public async Task<IActionResult> AddUrlAsync([FromBody] AddUrlCommand command, CancellationToken cancellationToken = default!)
    {
        var result = await _mediator.Send(command, cancellationToken);
        // Logic to handle the request and return a response
        return Ok(result.Value);
    }

    [HttpGet("{shortCode}")]
    public async Task<IActionResult> GetOriginalUrlAsync([FromRoute]string shortCode , CancellationToken cancellationToken = default!)
    {
        // 1. إرسال الـ Query عشان نجيب الرابط (من الكاش أو الداتابيز)
        var query = new GetOriginalUrlQuery(shortCode);
        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return Redirect("/not-found.html");

        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

        // سحب الـ User-Agent (نوع المتصفح والجهاز)
        var userAgent = Request.Headers.UserAgent.ToString();

        var visitedEvent = new UrlVisitedEvent(shortCode, ipAddress, userAgent);

        await _mediator.Publish(visitedEvent, cancellationToken);

        return Redirect(result.Value.Url);
    }
}
