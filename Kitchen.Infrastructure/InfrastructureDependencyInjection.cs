using Kitchen.Core.Repositories;
using Kitchen.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IIngredientRepository, InMemoryIngredientRepository>();
        services.AddSingleton<IIngredientTypeRepository, InMemoryIngredientTypeRepository>();
        return services;
    }
}