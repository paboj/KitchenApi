using Kitchen.Core.Domain.Entities;
using Kitchen.Core.Domain.Enums;
using Kitchen.Core.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kitchen.Infrastructure.DAL.Configurations
{
    internal sealed class IngredientTypeConfiguration : IEntityTypeConfiguration<IngredientType>
    {
        public void Configure(EntityTypeBuilder<IngredientType> builder)
        {
            builder.HasKey(x => x.Name);
            builder.Property(x => x.Name)
                .HasConversion(x => x.Value, x => new IngredientName(x));

            builder.Property(x => x.Unit)
                .HasConversion(
                    x => (int)x,
                    x => (UnitType)x
                );

            builder.Property(x => x.Category)
                .IsRequired()
                .HasConversion(
                    x => (int)x,
                    x => (Category)x
        );
        }
    }
}
