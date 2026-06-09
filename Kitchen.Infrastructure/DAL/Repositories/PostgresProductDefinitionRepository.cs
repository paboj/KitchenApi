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

        public void Add(ProductDefinition productDefinition)
        {
            _dbContext.ProductDefinitions.Add(productDefinition);
            _dbContext.SaveChanges();
        }

        public void Update(ProductDefinition productDefinition)
        {
            _dbContext.ProductDefinitions.Update(productDefinition);
            _dbContext.SaveChanges();
        }

        public void Delete(string name)
        {
            var entity = GetByName(name);
            if (entity is not null)
            {
                _dbContext.ProductDefinitions.Remove(entity);
                _dbContext.SaveChanges();
            }
        }

        public IEnumerable<ProductDefinition> GetAll()
            => _dbContext.ProductDefinitions
            .AsNoTracking()
            .ToList();

        public ProductDefinition? GetByName(string name)
            => _dbContext.ProductDefinitions
                .AsNoTracking()
                .SingleOrDefault(x => x.Name == new ProductName(name));
    }
}
