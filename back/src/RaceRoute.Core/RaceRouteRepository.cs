
using Microsoft.EntityFrameworkCore;
using RaceRoute.Core.Context;
using RaceRoute.Core.Domain;
using RaceRoute.Core.Dto;

namespace RaceRoute.Core;

public class GenerateArgs
{
    public double HeightMean { get; set; }
    public double HeightStddev { get; set; }
    public double DistanceMean { get; set; }
    public double DistanceStddev { get; set; }
    public int MaxPoints { get; set; }
}

public interface IRaceRouteRepository
{
    Task<RaceDto[]> GetRaces(CancellationToken cancellation);
    Task<(PointDto[] Points, TrackDto[] Tracks)> GetRaceInfo(int raceId, CancellationToken cancellation);
    Task<RaceDto> GenerateNewRace(GenerateArgs args, CancellationToken cancellation);
    Task<RaceDto> RemoveRace(int raceId, CancellationToken cancellation);
}

public class RaceRouteRepository : IRaceRouteRepository
{
    private readonly RaceRouteDbContext dbContext;

    public RaceRouteRepository(RaceRouteDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<RaceDto> GenerateNewRace(GenerateArgs args, CancellationToken cancellation)
    {
        if (args.MaxPoints <= 10)
            throw new ArgumentException($"Too few MaxPoints. Must be more than 10, but was {args.MaxPoints}");

        if (args.HeightMean <= 0.001)
            throw new ArgumentException($"Too small or negative HightMean {args.HeightMean}");

        if (args.HeightStddev <= 0.001)
            throw new ArgumentException($"Too small or negative HeightStddev {args.HeightStddev}");

        if (args.DistanceMean <= 0.001)
            throw new ArgumentException($"Too small or negative DistancetMean {args.DistanceMean}");

        if (args.DistanceStddev <= 0.001)
            throw new ArgumentException($"Too small or negative DistanceStddev {args.DistanceStddev}");

        var maxSpeedCount = Enum.GetValues(typeof(MaxSpeed)).Length;
        var surfaceCount = Enum.GetValues(typeof(Surface)).Length;
        var rnd = new Random();
        var nTracks = rnd.Next(args.MaxPoints - 1);
        var pointsCount = await dbContext.Points.CountAsync(cancellation);

        var firstPoint = new Point { Name = $"Point #{++pointsCount}", Height = SampleGaussian(rnd, args.HeightMean, args.HeightStddev) };
        var surface = (Surface)rnd.Next(surfaceCount);
        var tracks = Enumerable.Range(0, nTracks)
                    .Select(i =>
                    {
                        var secondPoint = new Point { Name = $"Point #{++pointsCount}", Height = SampleGaussian(rnd, args.HeightMean, args.HeightStddev) };
                        var track = new Track
                        {
                            Distance = SampleGaussian(rnd, args.DistanceMean, args.DistanceStddev),
                            First = firstPoint,
                            Second = secondPoint,
                            MaxSpeed = (MaxSpeed)rnd.Next(maxSpeedCount),
                            Surface = surface,
                        };
                        firstPoint = secondPoint;
                        if (rnd.Next(10) > 7)
                        {
                            surface = (Surface)(((int)surface + rnd.Next(surfaceCount - 1) + 1) % surfaceCount);
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

    public async Task<(PointDto[] Points, TrackDto[] Tracks)> GetRaceInfo(int raceId, CancellationToken cancellation)
    {
        var ret = await dbContext.Races
            .Where(r => r.Id == raceId)
            .Select(r => new { r.Tracks, Points = r.Tracks.Select(t => t.First).Concat(r.Tracks.Select(t => t.Second)).Distinct() })
            .FirstOrDefaultAsync(cancellation);
        if (ret == null)
            throw new ArgumentException($"Race '{raceId}' doesnt exists");

        var tracks = ret.Tracks.Select(TrackDto.From).ToArray();
        var points = ret.Points.Select(PointDto.From).ToArray();

        return (points, tracks);
    }

    public async Task<RaceDto[]> GetRaces(CancellationToken cancellation)
    {
        var races = await dbContext
            .Races
            .ToArrayAsync(cancellation);
        return races.Select(RaceDto.From).ToArray();
    }

    public async Task<RaceDto> RemoveRace(int raceId, CancellationToken cancellation)
    {
        var toBeDeleted = await dbContext.Races.FindAsync(raceId, cancellation);
        if (toBeDeleted == null)
            throw new ArgumentException($"Race '{raceId}' doesnt exists");

        dbContext.Remove(toBeDeleted);
        await dbContext.SaveChangesAsync(cancellation);
        return RaceDto.From(toBeDeleted);
    }
}