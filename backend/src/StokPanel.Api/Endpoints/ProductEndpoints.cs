using StokPanel.Application.Features.Products;
using StokPanel.Application.Features.StockMovements;
using StokPanel.Application.Security;

namespace StokPanel.Api.Endpoints
{
    public static class ProductEndpoints
    {
        public static void MapProductEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/products").WithTags("Ürünler").RequireAuthorization();

            group.MapGet("/", ([AsParameters] ProductFilter filter, GetProductsHandler h, CancellationToken ct)
                => h.GetProductsHandleAsync(filter, ct));

            group.MapGet("/{id:guid}", (Guid id, GetProductHandler h, CancellationToken ct) => h.GetProductHandleAsync(id, ct));

            group.MapPost("/", async (SaveProductRequest r, CreateProductHandler h, CancellationToken ct) =>
            {
                var id = await h.CreateProductsHandleAsync(r, ct);
                return Results.Created($"/api/products/{id}", new { id });
            }).RequireAuthorization(Policies.AdminOnly);

            group.MapPut("/{id:guid}", async (Guid id, SaveProductRequest r, UpdateProductHandler h, CancellationToken ct) =>
            {
                await h.UpdateProductsHandleAsync(id, r, ct);
                return Results.NoContent();
            }).RequireAuthorization(Policies.AdminOnly);

            group.MapPatch("/{id:guid}/active", async (Guid id, SetActiveRequest r, SetProductActiveHandler h, CancellationToken ct) =>
            {
                await h.SetProductHandleAsync(id, r, ct);
                return Results.NoContent();
            }).RequireAuthorization(Policies.AdminOnly);

            // Stok hareketleri: giriş yapmış herkes (Personel dahil)
            group.MapGet("/{id:guid}/movements", (Guid id, int? take, GetProductMovementsHandler h, CancellationToken ct)
                => h.HandleAsync(id, take ?? 50, ct));

            group.MapPost("/{id:guid}/movements", (Guid id, CreateMovementRequest r, CreateMovementHandler h, CancellationToken ct)
                => h.HandleAsync(id, r, ct));
        }
    }
}