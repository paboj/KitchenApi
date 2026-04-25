using Kitchen.Application.Services;
using Microsoft.Extensions.DependencyInjection;

public static class Extensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IInventoryService, InventoryService>();
        services.AddSingleton<ICatalogService, CatalogService>();

        return services;
    }
}
