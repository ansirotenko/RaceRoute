
namespace RaceRoute.Core.Domain;

public record TrackDto(int Id, int FirstId, int SecondId, double Distance, Surface Surface, MaxSpeed MaxSpeed)
{
    public static TrackDto From(Track track)
    {
        return new TrackDto(track.Id, track.First.Id, track.Second.Id, track.Distance, track.Surface, track.MaxSpeed);
    }
}