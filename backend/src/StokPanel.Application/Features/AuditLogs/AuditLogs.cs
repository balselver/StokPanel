using Microsoft.EntityFrameworkCore;
using StokPanel.Application.Abstraction;

namespace StokPanel.Application.Features.AuditLogs
{
    public sealed record AuditLogDto(Guid Id, string EntityType, string Action, string Description, string UserName, DateTime CreatedAtUtc);
    public sealed record AuditLogPage(List<AuditLogDto> Items, int Page, int TotalPages, int TotalCount);
    public sealed record AuditLogFilter(string? EntityType, int? Page);

    public sealed class GetAuditLogsHandler(IAppDbContext db, IIdentityService identityService)
    {
        private const int PageSize = 25;

        public async Task<AuditLogPage> HandleAsync(AuditLogFilter filter, CancellationToken ct)
        {
            var query = db.AuditLogs.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(filter.EntityType))
                query = query.Where(l => l.EntityType == filter.EntityType);

            var totalCount = await query.CountAsync(ct);
            var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)PageSize));
            var page = Math.Clamp(filter.Page ?? 1, 1, totalPages);

            var logs = await query
                .OrderByDescending(l => l.CreatedAtUtc)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync(ct);

            // Kayıtta e-posta saklı; ekranda güncel ad soyad gösterilsin
            var names = await identityService.GetUserNameAsync(logs.Where(l => l.UserId != null).Select(l => l.UserId!));

            var items = logs
                .Select(l => new AuditLogDto(l.Id, l.EntityType, l.Action, l.Description,
                    l.UserId is not null && names.TryGetValue(l.UserId, out var name) ? name : l.UserName,
                    l.CreatedAtUtc))
                .ToList();

            return new AuditLogPage(items, page, totalPages, totalCount);
        }
    }
}
