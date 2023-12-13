
using RaceRoute.Core.Domain;
using RaceRoute.Core.Dto;

namespace RaceRoute.Core;

public record GenerateArgs(double HeightMean, 
                           double HeightStddev, 
                           double DistanceMean, 
                           double DistanceStddev, 
                           double SurfaceSmoothness,
                           double SpeedSmoothness, 
                           int MaxPoints);

public record RaceInfoResult(PointDto[] Points, TrackDto[] Tracks);
public record RacesResult(RaceDto[] Races);

public interface IRaceRouteRepository
{
    Task<RacesResult> GetRaces(CancellationToken cancellation);
    Task<RaceInfoResult> GetRaceInfo(int raceId, CancellationToken cancellation);
    Task<RaceDto> GenerateNewRace(GenerateArgs args, CancellationToken cancellation);
    Task<RaceDto> RemoveRace(int raceId, CancellationToken cancellation);
}