using Kitchen.Core.Domain.Entities;
using Kitchen.Core.Domain.Enums;
using Kitchen.Infrastructure.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kitchen.Infrastructure.BackgroundServices
{
    internal class DatabaseInitBackgroundService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        public DatabaseInitBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<KitchenDbContext>();
                dbContext.Database.Migrate();

                var ingredients = dbContext.Ingredients.ToList();
                if (!ingredients.Any())
                {
                    ingredients = new List<Ingredient>()
                    {
                        new Ingredient("Test", 1, StorageLocation.Unspecified),
                        new Ingredient("Test - lodówka", 2, StorageLocation.Fridge),
                        new Ingredient("Test - zamrażarka", 5, StorageLocation.Freezer),
                        new Ingredient("Test - spiżarnia", 10, StorageLocation.Pantry)
                    };
                    dbContext.Ingredients.AddRange(ingredients);
                    dbContext.SaveChanges();
                }
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
