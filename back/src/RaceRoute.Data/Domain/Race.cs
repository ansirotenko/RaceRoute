
using System.ComponentModel.DataAnnotations;

namespace RaceRoute.Data.Domain;

public class Race 
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Track> Tracks { get; set; }
}