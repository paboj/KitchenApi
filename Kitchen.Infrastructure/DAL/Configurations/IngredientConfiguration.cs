using Kitchen.Core.Domain.Entities;
using Kitchen.Core.Domain.Enums;
using Kitchen.Core.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kitchen.Infrastructure.DAL.Configurations
{
    internal sealed class IngredientConfiguration : IEntityTypeConfiguration<Ingredient>
    {
        public void Configure(EntityTypeBuilder<Ingredient> builder) 
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .HasConversion(x => x.Value, x => new IngredientId(x));

            builder.Property(x => x.Name)
                .IsRequired()
                .HasConversion(x => x.Value, x => new IngredientName(x));
            builder.HasIndex(x => x.Name)
                .IsUnique();

            builder.Property(x => x.Amount);

            builder.Property(x => x.Location)
                .HasConversion(
                    x => (int)x,
                    x => (StorageLocation)x
                );
        }
    }
}
