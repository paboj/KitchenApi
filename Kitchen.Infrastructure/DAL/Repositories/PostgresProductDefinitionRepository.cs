using Kitchen.Core.Domain.Entities;
using Kitchen.Core.Repositories;
using Kitchen.Core.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Kitchen.Infrastructure.DAL.Repositories
{
    internal sealed class PostgresProductDefinitionRepository : IProductDefinitionRepository
    {
        private readonly KitchenDbContext _dbContext;

        public PostgresProductDefinitionRepository(KitchenDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(ProductDefinition productDefinition)
        {
            _dbContext.ProductDefinitions.Add(productDefinition);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(ProductDefinition productDefinition)
        {
            _dbContext.ProductDefinitions.Update(productDefinition);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Delete(string name)
        {
            var entity = await GetByName(name);
            if (entity is not null)
            {
                _dbContext.ProductDefinitions.Remove(entity);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ProductDefinition>> GetAll()
            => await _dbContext.ProductDefinitions
            .AsNoTracking()
            .ToListAsync();

        public async Task<ProductDefinition?> GetByName(string name)
            => await _dbContext.ProductDefinitions
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Name == new ProductName(name));
    }
}
