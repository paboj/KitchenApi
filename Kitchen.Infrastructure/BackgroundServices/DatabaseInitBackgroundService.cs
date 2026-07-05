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

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<KitchenDbContext>();
                await dbContext.Database.MigrateAsync();

                if (!await dbContext.StockItems.AnyAsync())
                {
                    await SeedAsync(dbContext);
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        private static async Task SeedAsync(KitchenDbContext dbContext)
        {
            var milk = new ProductDefinition("Mleko", UnitType.Liters, Category.Dairy);
            var eggs = new ProductDefinition("Jajka", UnitType.Pieces, Category.Dairy);
            var chicken = new ProductDefinition("Kurczak", UnitType.Kilograms, Category.Meat);
            var carrot = new ProductDefinition("Marchew", UnitType.Kilograms, Category.Vegetables);
            var rice = new ProductDefinition("Ryż", UnitType.Kilograms, Category.DryGoods);
            var paprika = new ProductDefinition("Papryka mielona", UnitType.Pieces, Category.Spices);

            dbContext.ProductDefinitions.AddRange(milk, eggs, chicken, carrot, rice, paprika);
            await dbContext.SaveChangesAsync();

            dbContext.StockItems.AddRange(
                new StockItem("Mleko", 2, StorageLocation.Fridge, milk),
                new StockItem("Jajka", 10, StorageLocation.Fridge, eggs),
                new StockItem("Kurczak", 1.5, StorageLocation.Freezer, chicken),
                new StockItem("Marchew", 1, StorageLocation.Pantry, carrot),
                new StockItem("Ryż", 2, StorageLocation.Pantry, rice),
                new StockItem("Papryka mielona", 1, StorageLocation.Pantry, paprika)
            );
            await dbContext.SaveChangesAsync();
        }
    }
}
