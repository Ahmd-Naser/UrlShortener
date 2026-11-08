using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application.Url.Commands.AddUrl;

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
        return Ok(result.Url);
    }

}
