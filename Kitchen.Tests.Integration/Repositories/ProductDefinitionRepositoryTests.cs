using AwesomeAssertions;
using Kitchen.Core.Domain.Entities;
using Kitchen.Core.Domain.Enums;
using Kitchen.Core.ValueObjects;
using Kitchen.Infrastructure.DAL;
using Kitchen.Infrastructure.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace Kitchen.Tests.Integration.Repositories
{
    public class ProductDefinitionRepositoryTests : IAsyncLifetime
    {
        private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:16-alpine")
            .Build();

        private KitchenDbContext _dbContext = null!;
        private PostgresProductDefinitionRepository _repository = null!;

        public async Task InitializeAsync()
        {
            await _postgres.StartAsync();

            var options = new DbContextOptionsBuilder<KitchenDbContext>()
                .UseNpgsql(_postgres.GetConnectionString())
                .Options;

            _dbContext = new KitchenDbContext(options);

            // MigrateAsync to test migrations, not EnsureCreated()
            await _dbContext.Database.MigrateAsync();

            _repository = new PostgresProductDefinitionRepository(_dbContext);
        }

        public async Task DisposeAsync()
        {
            await _dbContext.DisposeAsync();
            await _postgres.DisposeAsync();
        }

        [Fact]
        public async Task Add_ShouldPersistProductDefinition_AndBeRetrievableByName()
        {
            var definition = new ProductDefinition("Mleko", UnitType.Liters, Category.Dairy);

            await _repository.Add(definition);

            var fetched = await _repository.GetByName("Mleko");

            fetched.Should().NotBeNull();
            fetched!.Name.Should().Be(new ProductName("Mleko"));
            fetched.Unit.Should().Be(UnitType.Liters);
            fetched.Category.Should().Be(Category.Dairy);
        }
    }
}
