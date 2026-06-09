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

                var stockItems = dbContext.StockItems.ToList();
                if (!stockItems.Any())
                {
                    stockItems = new List<StockItem>()
                    {
                        new StockItem("Test", 1, StorageLocation.Unspecified, null),
                        new StockItem("Test - lodówka", 2, StorageLocation.Fridge, null),
                        new StockItem("Test - zamrażarka", 5, StorageLocation.Freezer, null),
                        new StockItem("Test - spiżarnia", 10, StorageLocation.Pantry, null)
                    };
                    dbContext.StockItems.AddRange(stockItems);
                    dbContext.SaveChanges();
                }
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
