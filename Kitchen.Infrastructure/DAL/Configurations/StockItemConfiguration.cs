using Kitchen.Core.Domain.Entities;
using Kitchen.Core.Domain.Enums;
using Kitchen.Core.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kitchen.Infrastructure.DAL.Configurations
{
    internal sealed class StockItemConfiguration : IEntityTypeConfiguration<StockItem>
    {
        public void Configure(EntityTypeBuilder<StockItem> builder) 
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .HasConversion(x => x.Value, x => new StockItemId(x));

            builder.Property(x => x.Name)
                .IsRequired()
                .HasConversion(x => x.Value, x => new ProductName(x));

            builder.Property(x => x.Amount);

            builder.Property(x => x.Location)
                .HasConversion(
                    x => (int)x,
                    x => (StorageLocation)x
                );

            builder.Property<ProductName>("TypeName")
            .HasConversion(x => x.Value, x => new ProductName(x))
            .IsRequired(false);

            builder.HasOne(x => x.Type)
                .WithMany()
                .HasForeignKey("TypeName")
                .IsRequired(false);
        }
    }
}
