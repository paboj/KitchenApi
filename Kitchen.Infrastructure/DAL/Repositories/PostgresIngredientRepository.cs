using Kitchen.Core.Domain.Entities;
using Kitchen.Core.Repositories;
using Kitchen.Core.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Kitchen.Infrastructure.DAL.Repositories
{
    internal sealed class PostgresIngredientRepository : IIngredientRepository
    {
        private readonly KitchenDbContext _dbContext;

        public PostgresIngredientRepository(KitchenDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Ingredient> GetAll()
            => _dbContext.Ingredients
            .AsNoTracking()
            .ToList();
        

        public Ingredient? GetByName(string name)
            => _dbContext.Ingredients
            .AsNoTracking()
            .FirstOrDefault(x => x.Name == new IngredientName(name));

        public void Add(Ingredient ingredient)
        {
            _dbContext.Ingredients.Add(ingredient);
            _dbContext.SaveChanges();
        }

        public void Update(Ingredient ingredient)
        {
            _dbContext.Ingredients.Update(ingredient);
            _dbContext.SaveChanges();
        }

        public void Delete(string name)
        {
            var ingredient = GetByName(name);

            if (ingredient != null)
            {
                _dbContext.Ingredients.Remove(ingredient);
                _dbContext.SaveChanges();
            }
        }
        public IEnumerable<Ingredient> GetAllWithDetails()
            => _dbContext.Ingredients
            .Include(i => i.Type)
            .AsNoTracking()
            .ToList();


        public Ingredient? GetByNameWithDetails(string name)
            => _dbContext.Ingredients
            .Include(i => i.Type)
            .AsNoTracking()
            .FirstOrDefault(x => x.Name == new IngredientName(name));
    }
}
