using Kitchen.Core.Domain.Entities;
using Kitchen.Core.Repositories;
using Kitchen.Core.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Kitchen.Infrastructure.DAL.Repositories
{
    internal sealed class PostgresIngredientTypeRepository : IIngredientTypeRepository
    {
        private readonly KitchenDbContext _dbContext;

        public PostgresIngredientTypeRepository(KitchenDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(IngredientType ingredientType)
        {
            _dbContext.IngredientTypes.Add(ingredientType);
            _dbContext.SaveChanges();
        }

        public void Delete(string name)
        {
            var entity = GetByName(name);
            if (entity is not null)
            {
                _dbContext.IngredientTypes.Remove(entity);
                _dbContext.SaveChanges();
            }
        }

        public IEnumerable<IngredientType> GetAll()
            => _dbContext.IngredientTypes.AsNoTracking().ToList();

        public IngredientType? GetByName(string name)
            => _dbContext.IngredientTypes
                .AsNoTracking()
                .SingleOrDefault(x => x.Name == new IngredientName(name));
    }
}
