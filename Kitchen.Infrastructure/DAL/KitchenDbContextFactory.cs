using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Kitchen.Infrastructure.DAL
{
    // TODO: helper to find dbContext
    internal sealed class KitchenDbContextFactory : IDesignTimeDbContextFactory<KitchenDbContext>
    {
        // Same UserSecretsId as Kitchen.Api's .csproj — reuses the same locally-set secret,
        // no DI container available here since this only runs for design-time EF tooling.
        private const string UserSecretsId = "b4cd8494-5328-43c4-ae16-85fadbfe84fe";

        public KitchenDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets(UserSecretsId)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration["database:ConnectionString"];

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "database:ConnectionString is not set. Run 'dotnet user-secrets set \"database:ConnectionString\" \"Host=...;Database=...;Username=...;Password=...\" --project Kitchen.Api' first.");
            }

            var optionsBuilder = new DbContextOptionsBuilder<KitchenDbContext>();

            optionsBuilder.UseNpgsql(connectionString);

            return new KitchenDbContext(optionsBuilder.Options);
        }
    }
}
