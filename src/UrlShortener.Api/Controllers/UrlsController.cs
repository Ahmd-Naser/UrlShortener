using MediatR;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Api.Extensions;
using UrlShortener.Application.Url.Commands.AddUrl;
using UrlShortener.Application.Url.Queries.GetOriginalUrl;

namespace UrlShortener.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UrlsController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpPost]
    public async Task<IActionResult> AddUrlAsync([FromBody] AddUrlCommand command)
    {
        var result = await _sender.Send(command);
        // Logic to handle the request and return a response
        return Ok(result.Value);
    }

    [HttpGet("{shortCode}")]
    public async Task<IActionResult> GetOriginalUrlAsync([FromRoute]string shortCode)
    {
        // Logic to retrieve the original URL based on the short code
        // For example, you might call a service method here
        var result = await _sender.Send(new GetOriginalUrlQuery(shortCode));

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
}
