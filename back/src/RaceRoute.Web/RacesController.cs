using Microsoft.AspNetCore.Mvc;
using RaceRoute.Core;
using RaceRoute.Core.Dto;

namespace RaceRoute.Web;

[ApiController]
[Route("api/races")]
public class RacesController : ControllerBase
{
    private readonly IRaceRouteRepository raceRouteRepository;

    public RacesController(IRaceRouteRepository raceRouteRepository)
    {
        this.raceRouteRepository = raceRouteRepository;
    }
    
    [HttpGet]
    public Task<RacesResult> GetRaces(CancellationToken cancellationToken)
    {
        return raceRouteRepository.GetRaces(cancellationToken);
    }

    [HttpGet("{raceId:int}")]
    public Task<RaceInfoResult> GetRaceInfo(int raceId, CancellationToken cancellationToken)
    {
        return raceRouteRepository.GetRaceInfo(raceId, cancellationToken);
    }

    [HttpPut]
    public Task<RaceDto> GenerateNewRace(GenerateArgs args, CancellationToken cancellationToken)
    {
        return raceRouteRepository.GenerateNewRace(args, cancellationToken);
    }

    [HttpDelete("{raceId:int}")]
    public async Task Delete(int raceId, CancellationToken cancellationToken)
    {
        await raceRouteRepository.RemoveRace(raceId, cancellationToken);
    }
}