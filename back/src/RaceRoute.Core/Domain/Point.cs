
using System.ComponentModel.DataAnnotations;

namespace RaceRoute.Core.Domain;

public class Point 
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public double Height { get; set; }
    public List<Track> AsFirstTracks { get; set; }
    public List<Track> AsSecondTracks { get; set; }
}
