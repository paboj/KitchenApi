using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kitchen.Core.Repositories;
using Kitchen.Infrastructure.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Kitchen.Infrastructure.DAL
{
    internal static class Extensions
    {
        public static IServiceCollection AddPostgres(this IServiceCollection services)
        {
            const string connectionString = "Host=localhost;Database=Kitchen;Username=postgres;Password=postgres"; //TODO: temporary
            services.AddDbContext<KitchenDbContext>(x => x.UseNpgsql(connectionString));
            services.AddScoped<IIngredientRepository, PostgresIngredientRepository>();
            services.AddScoped<IIngredientTypeRepository, PostgresIngredientTypeRepository>();

            return services;
        }
    }
}
