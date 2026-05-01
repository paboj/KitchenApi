using Kitchen.Infrastructure.BackgroundServices;
using Kitchen.Infrastructure.DAL;
using Microsoft.Extensions.DependencyInjection;

public static class Extensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddPostgres();
        services.AddHostedService<DatabaseInitBackgroundService>();

        return services;
    }
}