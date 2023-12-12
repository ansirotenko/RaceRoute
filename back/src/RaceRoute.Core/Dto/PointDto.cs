
using RaceRoute.Core.Domain;

namespace RaceRoute.Core.Dto;

public record PointDto(int Id, string Name, double Height)
{
    public static PointDto From(Point point) 
    {
        return new PointDto(point.Id, point.Name, point.Height);
    }
}
