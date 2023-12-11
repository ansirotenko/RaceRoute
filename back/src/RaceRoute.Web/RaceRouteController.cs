using Microsoft.AspNetCore.Mvc;

namespace RaceRoute.Web;

[ApiController]
[Route("api")]
public class RaceRouteController : ControllerBase
{
    [HttpGet("raceRoutes")]
    public Task<string[]> All()
    {
        return Task.FromResult(new string[]{"a", "b"});
    }

    [HttpGet("raceRoutes/{id}")]
    public Task<string> Single(string id)
    {
        return Task.FromResult(id);
    }

    [HttpPut("raceRoutes")]
    public Task Add(string id)
    {
        return Task.CompletedTask;
    }

    [HttpDelete("raceRoutes")]
    public Task Delete(string id)
    {
        return Task.CompletedTask;
    }
}
