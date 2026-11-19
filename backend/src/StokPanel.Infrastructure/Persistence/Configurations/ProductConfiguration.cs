using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StokPanel.Domain.Categories;
using StokPanel.Domain.Products;

namespace StokPanel.Infrastructure.Persistence.Configurations
{
    internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).ValueGeneratedNever();

            builder.Property(p => p.Name)
                .HasMaxLength(Product.NameMaxLength)
                .IsRequired();

            builder.Property(p => p.Sku)
                .HasMaxLength(Product.SkuMaxLength)
                .IsRequired();

            builder.HasIndex(p => p.Sku).IsUnique();

            builder.Property(p => p.Unit)
                .HasMaxLength(Product.UnitMaxLength)
                .IsRequired();

            builder.Property(p => p.Quantity).HasPrecision(18, 3);
            builder.Property(p => p.CriticalLevel).HasPrecision(18, 3);


            builder.HasOne<Category>()
                .WithMany()
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(p => p.IsCritical);
            builder.Ignore(p => p.DomainEvents);
        }
    }
}
