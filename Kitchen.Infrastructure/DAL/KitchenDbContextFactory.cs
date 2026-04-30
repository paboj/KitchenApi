using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            const string connectionString = "Host=localhost;Database=Kitchen;Username=postgres;Password=postgres";

            optionsBuilder.UseNpgsql(connectionString);

            return new KitchenDbContext(optionsBuilder.Options);
        }
    }
}
