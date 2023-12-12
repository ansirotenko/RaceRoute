
using RaceRoute.Core.Domain;

namespace RaceRoute.Core.Dto;

public record RaceDto(int Id, string Name)
{
    public static RaceDto From(Race race)
    {
        return new RaceDto(race.Id, race.Name);
    }
}