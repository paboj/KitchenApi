using System;
using System.Collections.Generic;
using System.Linq;
using Kitchen.Application.Services;
using Microsoft.Extensions.DependencyInjection;

public static class ApplicationDependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IInventoryService, InventoryService>();
        services.AddSingleton<ICatalogService, CatalogService>();

        return services;
    }
}
