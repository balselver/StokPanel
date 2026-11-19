using Microsoft.EntityFrameworkCore;
using StokPanel.Application.Abstraction;
using StokPanel.Application.Common;
using StokPanel.Domain.Common;
using StokPanel.Domain.StockMovements;

namespace StokPanel.Application.Features.StockMovements
{
    public sealed record CreateMovementRequest(StockMovementType Type, decimal Quantity, string? Note);

    public sealed record MovementDto(
        Guid Id, StockMovementType Type, decimal Quantity, decimal QuantityAfter,
        string? Note, string UserName, DateTime CreatedAtUtc);

    public sealed class CreateMovementHandler(IAppDbContext db, ICurrentUser currentUser)
    {
        public async Task<MovementDto> HandleAsync(Guid productId, CreateMovementRequest r, CancellationToken ct)
        {
            var product = await db.Products.FirstOrDefaultAsync(p => p.Id == productId, ct)
                ?? throw new NotFoundException("Ürün bulunamadı");

            var userId = currentUser.UserId ?? throw new DomainException("Oturum bulunamadı");

            var movement = r.Type switch
            {
                StockMovementType.In => StockMovement.CreateIn(product, r.Quantity, userId, r.Note),
                StockMovementType.Out => StockMovement.CreateOut(product, r.Quantity, userId, r.Note),
                _ => throw new DomainException("Geçersiz hareket türü")
            };

            db.StockMovements.Add(movement);
            await db.SaveChangesAsync(ct);   // Ürün + hareket aynı transaction'da

            return new MovementDto(movement.Id, movement.Type, movement.Quantity, movement.QuantityAfter,
                movement.Note, currentUser.UserName ?? string.Empty, movement.CreatedAtUtc);
        }
    }

    public sealed class GetProductMovementsHandler(IAppDbContext db, IIdentityService identityService)
    {
        public async Task<List<MovementDto>> HandleAsync(Guid productId, int take, CancellationToken ct)
        {
            if (!await db.Products.AnyAsync(p => p.Id == productId, ct))
                throw new NotFoundException("Ürün bulunamadı");

            var movements = await db.StockMovements.AsNoTracking()
                .Where(m => m.ProductId == productId)
                .OrderByDescending(m => m.CreatedAtUtc)
                .Take(Math.Clamp(take, 1, 200))
                .ToListAsync(ct);

            var names = await identityService.GetUserNameAsync(movements.Select(m => m.UserId));

            return movements
                .Select(m => new MovementDto(m.Id, m.Type, m.Quantity, m.QuantityAfter, m.Note,
                    names.GetValueOrDefault(m.UserId, "Bilinmeyen kullanıcı"), m.CreatedAtUtc))
                .ToList();
        }
    }
}