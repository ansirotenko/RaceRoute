using Microsoft.AspNetCore.Mvc;
using RaceRoute.Core;
using RaceRoute.Core.Domain;
using RaceRoute.Core.Dto;

namespace RaceRoute.Web;

[ApiController]
[Route("api")]
public class RaceRouteController : ControllerBase
{
    private readonly IRaceRouteRepository raceRouteRepository;

    public RaceRouteController(IRaceRouteRepository raceRouteRepository)
    {
        this.raceRouteRepository = raceRouteRepository;
    }

    [HttpGet("raceRoutes")]
    public Task<RaceDto[]> GetRaces(CancellationToken cancellationToken)
    {
        return raceRouteRepository.GetRaces(cancellationToken);
    }

    [HttpGet("raceRoutes/{id}")]
    public Task<(PointDto[] Points, TrackDto[] Tracks)> GetRaceInfo(int raceId, CancellationToken cancellationToken)
    {
        return raceRouteRepository.GetRaceInfo(raceId, cancellationToken);
    }

    [HttpPut("raceRoutes")]
    public Task<RaceDto> GenerateNewRace(GenerateArgs args, CancellationToken cancellationToken)
    {
        return raceRouteRepository.GenerateNewRace(args, cancellationToken);
    }

    [HttpDelete("raceRoutes")]
    public async Task Delete(int raceId, CancellationToken cancellationToken)
    {
        await raceRouteRepository.RemoveRace(raceId, cancellationToken);
    }
}
