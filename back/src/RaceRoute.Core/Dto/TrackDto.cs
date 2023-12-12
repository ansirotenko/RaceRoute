
namespace RaceRoute.Core.Domain;

public record TrackDto(int Id, int FirstId, int SecondId, double Distance, Surface Surface, MaxSpeed MaxSpeed)
{
    public static TrackDto From(Track track)
    {
        return new TrackDto(track.Id, track.FirstId, track.SecondId, track.Distance, track.Surface, track.MaxSpeed);
    }
}