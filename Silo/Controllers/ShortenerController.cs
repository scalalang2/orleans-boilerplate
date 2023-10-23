using GrainInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace Silo.Controllers;

[ApiController]
public class ShortenerController : Controller
{
    private IGrainFactory _grains;

    public ShortenerController(IGrainFactory grains)
    {
        _grains = grains;
    }
    
    [HttpGet]
    [Route("/shorten")]
    public async Task<IActionResult> Shorten([FromQuery] string url)
    {
        var host = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}";
        if (string.IsNullOrWhiteSpace(url) && Uri.IsWellFormedUriString(url, UriKind.Absolute) is false)
        {
            return BadRequest($"The URL query string is required and needs to be well formed.");
        }

        var shortenedRouteSegment = Guid.NewGuid().GetHashCode().ToString("X");
        var grain = _grains.GetGrain<IUrlShortnerGrain>(shortenedRouteSegment);
        await grain.SetUrl(url);
        var resultBuilder = new UriBuilder(host)
        {
            Path = $"/go/{shortenedRouteSegment}"
        };
        return Ok(resultBuilder.Uri);
    }
    
    [HttpGet]
    [Route("/go/{shortenedRouteSegment:required}")]
    public async Task<IActionResult> Go(string shortenedRouteSegment)
    {
        var grain = _grains.GetGrain<IUrlShortnerGrain>(shortenedRouteSegment);
        var url = await grain.GetUrl();
        if (string.IsNullOrWhiteSpace(url))
        {
            return NotFound();
        }

        return Redirect(url);
    }
}