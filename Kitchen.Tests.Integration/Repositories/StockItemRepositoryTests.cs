using FluentAssertions;
using Kitchen.Core.Domain.Entities;
using Kitchen.Core.Domain.Enums;
using Kitchen.Infrastructure.DAL;
using Kitchen.Infrastructure.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace Kitchen.Tests.Integration.Repositories
{
    public class StockItemRepositoryTests : IAsyncLifetime
    {
        private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:16-alpine")
            .Build();

        private KitchenDbContext _dbContext = null!;
        private PostgresStockItemRepository _repository = null!;

        public async Task InitializeAsync()
        {
            await _postgres.StartAsync();

            var options = new DbContextOptionsBuilder<KitchenDbContext>()
                .UseNpgsql(_postgres.GetConnectionString())
                .Options;

            _dbContext = new KitchenDbContext(options);

            // MigrateAsync to test migrations, not EnsureCreated()
            await _dbContext.Database.MigrateAsync();

            _repository = new PostgresStockItemRepository(_dbContext);
        }

        public async Task DisposeAsync()
        {
            await _dbContext.DisposeAsync();
            await _postgres.DisposeAsync();
        }

        [Fact]
        public async Task Add_ShouldAllowAddingStockItem_WhenNameAlreadyExists()
        {
            var first = new StockItem("Mleko", amount: 1, StorageLocation.Fridge, type: null);
            var second = new StockItem("Mleko", amount: 2, StorageLocation.Pantry, type: null);

            await _repository.Add(first);

            var addingDuplicate = async () => await _repository.Add(second);

            await addingDuplicate.Should().NotThrowAsync(
                "StockItem.Name doesn't have to be unique");

            var stockItems = await _repository.GetByName("Mleko");
            stockItems.Should().HaveCount(2);
        }
    }
}
