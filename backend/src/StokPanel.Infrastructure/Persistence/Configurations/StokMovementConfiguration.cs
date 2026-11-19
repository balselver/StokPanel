using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StokPanel.Domain.Products;
using StokPanel.Domain.StockMovements;
using StokPanel.Infrastructure.Identity;


namespace StokPanel.Infrastructure.Persistence.Configurations
{
    internal sealed class StokMovementConfiguration : IEntityTypeConfiguration<StockMovement>
    {
        public void Configure(EntityTypeBuilder<StockMovement> builder)
        {
            builder.ToTable("Stock Movements");

            builder.HasKey(m => m.Id);
            builder.Property(m => m.Id).ValueGeneratedNever();

            builder.Property(m => m.Type).IsRequired();

            builder.Property(m => m.Quantity).HasPrecision(18, 3);
            builder.Property(m => m.QuantityAfter).HasPrecision(18, 3);

            builder.Property(m => m.Note)
                .HasMaxLength(StockMovement.NoteMaxLength);

            builder.Property(m => m.UserId)
                .HasMaxLength(450)
                .IsRequired();

            builder.HasOne<Product>()
                .WithMany()
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(m => new { m.ProductId, m.CreatedAtUtc });
            builder.HasIndex(m => m.CreatedAtUtc);
            


        

        }
    }
}
