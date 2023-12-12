using Microsoft.EntityFrameworkCore;
using RaceRoute.Core.Domain;

namespace RaceRoute.Core.Context;

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
            .HasForeignKey(t => t.FirstId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();

        modelBuilder.Entity<Point>()
            .HasMany(p => p.AsSecondTracks)
            .WithOne(t => t.Second)
            .HasForeignKey(t => t.SecondId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();
    }
}