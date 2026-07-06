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
            var first = new StockItem("Mleko", amount: 1, StorageLocation.Fridge, definition: null);
            var second = new StockItem("Mleko", amount: 2, StorageLocation.Pantry, definition: null);

            await _repository.Add(first);

            var addingDuplicate = async () => await _repository.Add(second);

            await addingDuplicate.Should().NotThrowAsync(
                "StockItem.Name doesn't have to be unique");

            var stockItems = await _repository.GetByName("Mleko");
            stockItems.Should().HaveCount(2);
        }

        // Regression test: ProductDefinition is loaded with AsNoTracking (e.g. via
        // PostgresProductDefinitionRepository.GetByName), so DbContext doesn't know
        // it already exists. Without attaching it first, DbSet.Add marked the whole
        // reachable graph as Added and EF tried to re-insert it, causing a PK
        // violation on ProductDefinitions.
        [Fact]
        public async Task Add_ShouldNotFail_WhenStockItemNameMatchesExistingProductDefinition()
        {
            var definition = new ProductDefinition("Mleko", UnitType.Liters, Category.Dairy);
            _dbContext.ProductDefinitions.Add(definition);
            await _dbContext.SaveChangesAsync();
            _dbContext.ChangeTracker.Clear();

            var untrackedDefinition = await _dbContext.ProductDefinitions
                .AsNoTracking()
                .SingleAsync(d => d.Name == "Mleko");

            var stockItem = new StockItem("Mleko", amount: 1, StorageLocation.Fridge, definition: untrackedDefinition);

            var adding = async () => await _repository.Add(stockItem);

            await adding.Should().NotThrowAsync(
                "the ProductDefinition already exists and should be linked, not re-inserted");

            var definitions = await _dbContext.ProductDefinitions.AsNoTracking().ToListAsync();
            definitions.Should().HaveCount(1);
        }

        // Same root cause as above, on the Update path: DbSet.Update marks an
        // untracked ProductDefinition as Modified instead of leaving it alone.
        [Fact]
        public async Task Update_ShouldNotFail_WhenStockItemHasExistingProductDefinition()
        {
            var definition = new ProductDefinition("Mleko", UnitType.Liters, Category.Dairy);
            _dbContext.ProductDefinitions.Add(definition);
            await _dbContext.SaveChangesAsync();
            _dbContext.ChangeTracker.Clear();

            var untrackedDefinition = await _dbContext.ProductDefinitions
                .AsNoTracking()
                .SingleAsync(d => d.Name == "Mleko");

            var stockItem = new StockItem("Mleko", amount: 1, StorageLocation.Fridge, definition: untrackedDefinition);
            await _repository.Add(stockItem);
            _dbContext.ChangeTracker.Clear();

            var fetched = await _repository.GetByIdWithDetails(stockItem.Id.Value);
            fetched!.AdjustAmount(5);

            var updating = async () => await _repository.Update(fetched);

            await updating.Should().NotThrowAsync();

            var definitions = await _dbContext.ProductDefinitions.AsNoTracking().ToListAsync();
            definitions.Should().HaveCount(1);
        }
    }
}
