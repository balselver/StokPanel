using StokPanel.Domain.Common;

namespace StokPanel.Domain.AuditLogs
{
    public static class AuditEntityTypes
    {
        public const string Category = "Kategori";
        public const string Product = "Ürün";
        public const string User = "Kullanıcı";
    }

    // "Kim, ne zaman, neyi yaptı" kaydı. Bir kez yazılır, hiç değişmez.
    public sealed class AuditLog : Entity
    {
        public const int DescriptionMaxLength = 500;

        public string EntityType { get; private set; } = string.Empty;
        public string Action { get; private set; } = string.Empty;
        public string EntityId { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public string? UserId { get; private set; }
        public string UserName { get; private set; } = string.Empty;
        public DateTime CreatedAtUtc { get; private set; }

        private AuditLog() { }

        public static AuditLog Create(string entityType, string action, string entityId, string description,
            string? userId, string? userName)
            => new()
            {
                EntityType = entityType,
                Action = action,
                EntityId = entityId,
                Description = description.Length > DescriptionMaxLength ? description[..DescriptionMaxLength] : description,
                UserId = userId,
                UserName = userName ?? "Sistem",   // Seed gibi HTTP isteği dışındaki işlemler
                CreatedAtUtc = DateTime.UtcNow
            };
    }
}