using StokPanel.Domain.AuditLogs;
using StokPanel.Domain.Categories;
using StokPanel.Domain.Common;
using StokPanel.Domain.Products;

namespace StokPanel.Infrastructure.Persistence
{
    // Domain olayını okunabilir bir işlem kaydına çevirir
    internal static class AuditLogFactory
    {
        public static AuditLog? Create(IDomainEvent domainEvent, string? userId, string? userName)
        {
            (string Type, string Action, Guid Id, string Description)? info = domainEvent switch
            {
                CategoryCreatedEvent e => (AuditEntityTypes.Category, "Eklendi", e.CategoryId, $"\"{e.Name}\" kategorisi eklendi"),
                CategoryUpdateEvent e => (AuditEntityTypes.Category, "Güncellendi", e.CategoryId, $"\"{e.Name}\" kategorisi güncellendi"),
                CategoryDeletedEvent e => (AuditEntityTypes.Category, "Silindi", e.CategoryId, $"\"{e.Name}\" kategorisi silindi"),

                ProductCreatedEvent e => (AuditEntityTypes.Product, "Eklendi", e.ProductId, $"\"{e.Name}\" ({e.Sku}) ürünü eklendi"),
                ProductUpdatedEvent e => (AuditEntityTypes.Product, "Güncellendi", e.ProductId, $"\"{e.Name}\" ({e.Sku}) ürünü güncellendi"),
                ProductActiveChangedEvent e => (AuditEntityTypes.Product,
                    e.IsActive ? "Aktif edildi" : "Pasife alındı", e.ProductId,
                    $"\"{e.Name}\" ürünü {(e.IsActive ? "aktif edildi" : "pasife alındı")}"),

                // Stok değişimleri StockMovements tablosunda zaten ayrıntılı kayıtlı
                _ => null
            };

            return info is { } i
                ? AuditLog.Create(i.Type, i.Action, i.Id.ToString(), i.Description, userId, userName)
                : null;
        }
    }
}
