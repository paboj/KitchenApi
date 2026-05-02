using Kitchen.Core.Repositories;
using Kitchen.Infrastructure.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kitchen.Infrastructure.DAL
{
    internal static class Extensions
    {
        public static IServiceCollection AddPostgres(this IServiceCollection services, IConfiguration configuration)
        {
            var options = configuration.GetOptions<PostgresOptions>("database");
            services.AddDbContext<KitchenDbContext>(x => x.UseNpgsql(options.ConnectionString));
            services.AddScoped<IIngredientRepository, PostgresIngredientRepository>();
            services.AddScoped<IIngredientTypeRepository, PostgresIngredientTypeRepository>();
            services.AddScoped<DatabaseInitializer>();

            return services;
        }
    }
}
