using Kitchen.Application.Services;
using Microsoft.Extensions.DependencyInjection;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IInventoryService, InventoryService>();
        services.AddScoped<ICatalogService, CatalogService>();

        return services;
    }
}
