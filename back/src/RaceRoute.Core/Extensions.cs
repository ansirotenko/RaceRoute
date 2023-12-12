
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RaceRoute.Core.Context;

namespace RaceRoute.Core;

public static class Extensions
{
    public static IServiceCollection AddRaceRouteDb(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetSection("ConnectionString").Value;
        services.AddDbContext<RaceRouteDbContext>(x => x.UseSqlServer(connectionString));

        services.AddHealthChecks()
            .AddDbContextCheck<RaceRouteDbContext>();

        services.AddScoped<IRaceRouteRepository, RaceRouteRepository>();
        return services;
    }

    public static IServiceProvider ExecuteDbMigrations(this IServiceProvider provider)
    {
        using (var scope = provider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<RaceRouteDbContext>();
            dbContext.Database.Migrate();
        }
        return provider;
    }
}