
using System.ComponentModel.DataAnnotations;

namespace RaceRoute.Data.Domain;

public class Track 
{
    [Key]
    public int Id { get; set; }
    public Point First { get; set; }
    public Point Second { get; set; }
    public double Distance { get; set; }
    public Surface Surface { get; set; }
    public MaxSpeed MaxSpeed { get; set; }
    public Race Race { get; set; }
}