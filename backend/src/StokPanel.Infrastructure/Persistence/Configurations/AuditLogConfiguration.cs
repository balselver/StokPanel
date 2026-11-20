using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StokPanel.Domain.AuditLogs;

namespace StokPanel.Infrastructure.Persistence.Configurations
{
    internal sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.ToTable("AuditLogs");

            builder.HasKey(l => l.Id);
            builder.Property(l => l.Id).ValueGeneratedNever();

            builder.Property(l => l.EntityType).HasMaxLength(50).IsRequired();
            builder.Property(l => l.Action).HasMaxLength(50).IsRequired();
            builder.Property(l => l.EntityId).HasMaxLength(450).IsRequired();
            builder.Property(l => l.Description).HasMaxLength(AuditLog.DescriptionMaxLength).IsRequired();
            builder.Property(l => l.UserId).HasMaxLength(450);   // Bilerek FK yok: log, olay anının fotoğrafı
            builder.Property(l => l.UserName).HasMaxLength(256).IsRequired();

            builder.HasIndex(l => l.CreatedAtUtc);
            builder.HasIndex(l => new { l.EntityType, l.CreatedAtUtc });
        }
    }
}
