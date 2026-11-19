using Microsoft.EntityFrameworkCore;
using StokPanel.Application.Abstraction;
using StokPanel.Application.Features.Products;
using StokPanel.Domain.StockMovements;

namespace StokPanel.Application.Features.Dashboard
{
    public sealed record RecentMovementDto(
        Guid Id, Guid ProductId, string ProductName, string Unit,
        StockMovementType Type, decimal Quantity, string UserName, DateTime CreatedAtUtc);

    public sealed record DashboardDto(
        int ActiveProductCount, int CriticalCount, int TodayInCount, int TodayOutCount,
        List<ProductDto> CriticalProducts, List<RecentMovementDto> RecentMovements);

    public sealed class GetDashboardHandler(IAppDbContext db, IIdentityService identityService)
    {
        public async Task<DashboardDto> HandleAsync(CancellationToken ct)
        {
            // Türkiye UTC+3 (yaz saati yok): bugünün 00:00'ının UTC karşılığı
            var todayStartUtc = DateTime.UtcNow.AddHours(3).Date.AddHours(-3);

            var activeProducts = db.Products.Where(p => p.IsActive);
            var activeCount = await activeProducts.CountAsync(ct);
            var criticalProducts = await activeProducts
                .Where(p => p.Quantity <= p.CriticalLevel)
                .ToDto(db)
                .ToListAsync(ct);

            var todayMovements = db.StockMovements.Where(m => m.CreatedAtUtc >= todayStartUtc);
            var todayIn = await todayMovements.CountAsync(m => m.Type == StockMovementType.In, ct);
            var todayOut = await todayMovements.CountAsync(m => m.Type == StockMovementType.Out, ct);

            var recent = await (
                from m in db.StockMovements
                join p in db.Products on m.ProductId equals p.Id
                orderby m.CreatedAtUtc descending
                select new { m.Id, m.ProductId, ProductName = p.Name, p.Unit, m.Type, m.Quantity, m.UserId, m.CreatedAtUtc }
            ).Take(10).ToListAsync(ct);

            var names = await identityService.GetUserNameAsync(recent.Select(r => r.UserId));

            var recentDtos = recent
                .Select(r => new RecentMovementDto(r.Id, r.ProductId, r.ProductName, r.Unit, r.Type, r.Quantity,
                    names.GetValueOrDefault(r.UserId, "Bilinmeyen kullanıcı"), r.CreatedAtUtc))
                .ToList();

            return new DashboardDto(activeCount, criticalProducts.Count, todayIn, todayOut, criticalProducts, recentDtos);
        }
    }
}