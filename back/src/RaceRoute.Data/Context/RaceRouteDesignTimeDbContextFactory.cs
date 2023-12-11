using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace RaceRoute.Data.Context;
public class DesignTimeAppDbContextFactory : IDesignTimeDbContextFactory<RaceRouteDbContext>
{
    public RaceRouteDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<RaceRouteDbContext>();
        optionsBuilder.UseSqlServer(args.Last());

        return new RaceRouteDbContext(optionsBuilder.Options);
    }
}