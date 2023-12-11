using Microsoft.EntityFrameworkCore;
using RaceRoute.Data.Domain;

namespace RaceRoute.Data.Context;

public class RaceRouteDbContext: DbContext
{
    public RaceRouteDbContext(DbContextOptions<RaceRouteDbContext> options)
        : base(options)
    {
        
    }

    public virtual DbSet<Race> Races { get; set; } = default;
    public virtual DbSet<Track> Tracks { get; set; } = default;
    public virtual DbSet<Point> Points { get; set; } = default;

    protected override void OnModelCreating(ModelBuilder modelBuilder)  
    {
        modelBuilder.Entity<Race>()
            .HasMany(r => r.Tracks)
            .WithOne(t => t.Race)
            .IsRequired();

        modelBuilder.Entity<Point>()
            .HasMany(p => p.AsFirstTracks)
            .WithOne(t => t.First)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();

        modelBuilder.Entity<Point>()
            .HasMany(p => p.AsSecondTracks)
            .WithOne(t => t.Second)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();
    }
}