using Kitchen.Infrastructure.BackgroundServices;
using Kitchen.Infrastructure.DAL;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class Extensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPostgres(configuration);
        services.AddHostedService<DatabaseInitBackgroundService>();

        return services;
    }
}