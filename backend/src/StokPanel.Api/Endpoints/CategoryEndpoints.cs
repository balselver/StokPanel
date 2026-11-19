using StokPanel.Application.Features.Categories;
using StokPanel.Application.Security;

namespace StokPanel.Api.Endpoints
{
    public static class CategoryEndpoints
    {
        public static void MapCategoryEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/categories").WithTags("Kategoriler").RequireAuthorization();

            group.MapGet("/", (GetCategoriesHandler h, CancellationToken ct) => h.GetCategoryHandleAsync(ct));

            group.MapPost("/", async (SaveCategoryRequest r, CreateCategoryHandler h, CancellationToken ct) =>
            {
                var id = await h.CreateCategoryHandleAsync(r, ct);
                return Results.Created($"/api/categories/{id}", new { id });
            }).RequireAuthorization(Policies.AdminOnly);

            group.MapPut("/{id:guid}", async (Guid id, SaveCategoryRequest r, UpdateCategoryHandler h, CancellationToken ct) =>
            {
                await h.UpdateCategoryHandleAsync(id, r, ct);
                return Results.NoContent();
            }).RequireAuthorization(Policies.AdminOnly);

            group.MapDelete("/{id:guid}", async (Guid id, DeleteCategoryHandler h, CancellationToken ct) =>
            {
                await h.DeleteCategoryHandleAsync(id, ct);
                return Results.NoContent();
            }).RequireAuthorization(Policies.AdminOnly);
        }
    }
}