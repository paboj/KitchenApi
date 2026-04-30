using Kitchen.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kitchen.Infrastructure.DAL
{
    internal sealed class KitchenDbContext : DbContext
    {
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<IngredientType> IngredientTypes { get; set; }

        public KitchenDbContext(DbContextOptions<KitchenDbContext> dbContextOptions) : base(dbContextOptions) 
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}
