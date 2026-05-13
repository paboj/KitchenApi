using Kitchen.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kitchen.Infrastructure.DAL
{
    internal sealed class KitchenDbContext : DbContext
    {
        public DbSet<StockItem> StockItems { get; set; }
        public DbSet<ProductDefinition> ProductDefinitions { get; set; }

        public KitchenDbContext(DbContextOptions<KitchenDbContext> dbContextOptions) : base(dbContextOptions) 
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}
