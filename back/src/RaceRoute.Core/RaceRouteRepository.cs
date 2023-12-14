
using Microsoft.EntityFrameworkCore;
using RaceRoute.Core.Context;
using RaceRoute.Core.Domain;
using RaceRoute.Core.Dto;

namespace RaceRoute.Core;

public class RaceRouteRepository : IRaceRouteRepository
{
    private readonly RaceRouteDbContext dbContext;

    public RaceRouteRepository(RaceRouteDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<RaceDto> GenerateNewRace(GenerateArgs args, CancellationToken cancellation)
    {
        if (args.PointsNumber <= 10)
            throw new ArgumentException($"Too few PointsNumber. Must be more than 10, but was {args.PointsNumber}");

        if (args.HeightMean <= 0.001)
            throw new ArgumentException($"Too small or negative HightMean {args.HeightMean}");

        if (args.HeightStddev <= 0.001)
            throw new ArgumentException($"Too small or negative HeightStddev {args.HeightStddev}");

        if (args.DistanceMean <= 0.001)
            throw new ArgumentException($"Too small or negative DistancetMean {args.DistanceMean}");

        if (args.DistanceStddev <= 0.001)
            throw new ArgumentException($"Too small or negative DistanceStddev {args.DistanceStddev}");

        if (args.SurfaceSmoothness <= 0.001 || args.SurfaceSmoothness >= 1)
            throw new ArgumentException($"SurfaceSmoothness must be in range 0 - 1, But was {args.SurfaceSmoothness}");

        if (args.SpeedSmoothness <= 0.001 || args.SpeedSmoothness >= 1)
            throw new ArgumentException($"SpeedSmoothness must be in range 0 - 1, But was {args.SpeedSmoothness}");

        var maxSpeedCount = Enum.GetValues(typeof(MaxSpeed)).Length;
        var surfaceCount = Enum.GetValues(typeof(Surface)).Length;
        var rnd = new Random();
        var nTracks = args.PointsNumber - 1;
        var pointsCount = await dbContext.Points.CountAsync(cancellation);

        var firstPoint = new Point { Name = $"Point #{++pointsCount}", Height = SampleGaussian(rnd, args.HeightMean, args.HeightStddev) };
        var surface = (Surface)rnd.Next(surfaceCount);
        var maxSpeed = (MaxSpeed)rnd.Next(maxSpeedCount);
        var tracks = Enumerable.Range(0, nTracks)
                    .Select(i =>
                    {
                        var secondPoint = new Point { Name = $"Point #{++pointsCount}", Height = SampleGaussian(rnd, args.HeightMean, args.HeightStddev) };
                        var track = new Track
                        {
                            Distance = Math.Sqrt(Math.Pow(SampleGaussian(rnd, args.DistanceMean, args.DistanceStddev),2) + Math.Pow(firstPoint.Height - secondPoint.Height, 2)),
                            First = firstPoint,
                            Second = secondPoint,
                            MaxSpeed = maxSpeed,
                            Surface = surface,
                        };
                        firstPoint = secondPoint;
                        if (rnd.NextDouble() > args.SurfaceSmoothness)
                        {
                            surface = (Surface)(((int)surface + rnd.Next(surfaceCount - 1) + 1) % surfaceCount);
                        }
                        if (rnd.NextDouble() > args.SpeedSmoothness)
                        {
                            maxSpeed = (MaxSpeed)(((int)maxSpeed + rnd.Next(maxSpeedCount - 1) + 1) % maxSpeedCount);
                        }
                        return track;
                    }).ToList();

        var racesCount = await dbContext.Races.CountAsync(cancellation);
        var race = new Race
        {
            Name = $"Race #{racesCount + 1}",
            Tracks = tracks
        };

        await dbContext.Races.AddAsync(race, cancellation);
        await dbContext.SaveChangesAsync(cancellation);

        return RaceDto.From(race);
    }

    public static double SampleGaussian(Random random, double mean, double stddev)
    {
        double x1 = 1 - random.NextDouble();
        double x2 = 1 - random.NextDouble();

        double y1 = Math.Sqrt(-2.0 * Math.Log(x1)) * Math.Cos(2.0 * Math.PI * x2);
        return y1 * stddev + mean;
    }

    public async Task<RaceInfoResult> GetRaceInfo(int raceId, CancellationToken cancellation)
    {
        var ret = await dbContext.Races
            .Where(r => r.Id == raceId)
            .Select(r => new { r.Tracks, Points = r.Tracks.Select(t => t.First).Union(r.Tracks.Select(t => t.Second)) })
            .FirstOrDefaultAsync(cancellation);
        if (ret == null)
            throw new ArgumentException($"Race '{raceId}' doesnt exists");

        var tracks = ret.Tracks.Select(TrackDto.From).ToArray();
        var points = ret.Points.Select(PointDto.From).ToArray();

        return new (points, tracks);
    }

    public async Task<RacesResult> GetRaces(CancellationToken cancellation)
    {
        var races = await dbContext
            .Races
            .OrderByDescending(r => r.Id)
            .ToArrayAsync(cancellation);
        return new RacesResult(races.Select(RaceDto.From).ToArray());
    }

    public async Task<RaceDto> RemoveRace(int raceId, CancellationToken cancellation)
    {
        var toBeDeleted = await dbContext.Races.FindAsync(raceId, cancellation);
        if (toBeDeleted == null)
            throw new ArgumentException($"Race '{raceId}' doesnt exists");

        var tracksQuery = dbContext.Tracks.Where(t => t.Race.Id == raceId);
        var pointIds = await tracksQuery.Select(t => t.FirstId)
                        .Union(tracksQuery.Select(t => t.SecondId))
                        .ToArrayAsync();
        dbContext.Remove(toBeDeleted);
        await dbContext.SaveChangesAsync(cancellation);
        dbContext.Points.RemoveRange(pointIds.Select(pid => new Point{Id = pid}));
        await dbContext.SaveChangesAsync(cancellation);
        return RaceDto.From(toBeDeleted);
    }
}