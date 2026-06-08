using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Kitchen.Infrastructure.DAL
{
    // TODO: helper to find dbContext
    internal sealed class KitchenDbContextFactory : IDesignTimeDbContextFactory<KitchenDbContext>
    {
        public KitchenDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<KitchenDbContext>();

            const string connectionString = "Host=localhost;Database=KitchenDb;Username=postgres;Password=postgres";

            optionsBuilder.UseNpgsql(connectionString);

            return new KitchenDbContext(optionsBuilder.Options);
        }
    }
}
