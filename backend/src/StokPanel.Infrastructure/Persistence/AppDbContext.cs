using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StokPanel.Application.Abstraction;
using StokPanel.Domain.AuditLogs;
using StokPanel.Domain.Categories;
using StokPanel.Domain.Common;
using StokPanel.Domain.Products;
using StokPanel.Domain.StockMovements;
using StokPanel.Infrastructure.Identity;

namespace StokPanel.Infrastructure.Persistence
{
    internal sealed class AppDbContext(DbContextOptions<AppDbContext> options, ICurrentUser currentUser)
        : IdentityDbContext<ApplicationUser>(options), IAppDbContext
    {
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<StockMovement> StockMovements => Set<StockMovement>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // 1) Olay biriktirmiş nesneleri bul (eklenen, değişen, silinen)
            var aggregates = ChangeTracker.Entries<AggregateRoot>()
                .Where(e => e.Entity.DomainEvents.Count > 0)
                .Select(e => e.Entity)
                .ToList();

            // 2) Olayları al ve nesnelerden temizle (ikinci SaveChanges'te tekrar yazılmasın)
            var domainEvents = aggregates.SelectMany(a => a.DomainEvents).ToList();
            foreach (var aggregate in aggregates)
                aggregate.ClearDomainEvents();

            // 3) Her olayı log kaydına çevir
            foreach (var domainEvent in domainEvents)
            {
                var log = AuditLogFactory.Create(domainEvent, currentUser.UserId, currentUser.UserName);
                if (log is not null)
                    AuditLogs.Add(log);
            }

            // 4) Değişiklik + loglar tek transaction'da
            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            // Bütün DateTime özelliklerine uygula
            configurationBuilder.Properties<DateTime>().HaveConversion<UtcDateTimeConverter>();
        }
    }
}
