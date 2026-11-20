using Microsoft.EntityFrameworkCore;
using StokPanel.Application.Abstraction;
using StokPanel.Application.Common;
using StokPanel.Domain.Categories;
using StokPanel.Domain.Common;

namespace StokPanel.Application.Features.Categories
{
    public sealed record CategoryDto(Guid Id, string Name, string? Description, int ProductCount);
    public sealed record SaveCategoryRequest(string Name, string? Description);

    public sealed class GetCategoriesHandler(IAppDbContext db)
    {
        public Task<List<CategoryDto>> GetCategoryHandleAsync(CancellationToken ct)
            => db.Categories
                .OrderBy(c => c.Name)
                .Select(c => new CategoryDto(c.Id, c.Name, c.Description, db.Products.Count(p => p.CategoryId == c.Id)))
                .ToListAsync(ct);
    }

    public sealed class CreateCategoryHandler(IAppDbContext db)
    {
        public async Task<Guid> CreateCategoryHandleAsync(SaveCategoryRequest request, CancellationToken ct)
        {
            var category = Category.Create(request.Name, request.Description);   // 1) Domain kuralları
            await CategoryRules.EnsureNameIsFreeAsync(db, category.Name, null, ct); // 2) Veritabanı kuralları

            db.Categories.Add(category);
            await db.SaveChangesAsync(ct);
            return category.Id;
        }
    }

    public sealed class UpdateCategoryHandler(IAppDbContext db)
    {
        public async Task UpdateCategoryHandleAsync(Guid id, SaveCategoryRequest request, CancellationToken ct)
        {
            var category = await db.Categories.FirstOrDefaultAsync(c => c.Id == id, ct)
                ?? throw new NotFoundException("Kategori bulunamadı");

            category.Update(request.Name, request.Description);
            await CategoryRules.EnsureNameIsFreeAsync(db, category.Name, category.Id, ct);

            await db.SaveChangesAsync(ct);
        }
    }

    public sealed class DeleteCategoryHandler(IAppDbContext db)
    {
        public async Task DeleteCategoryHandleAsync(Guid id, CancellationToken ct)
        {
            var category = await db.Categories.FirstOrDefaultAsync(c => c.Id == id, ct)
                ?? throw new NotFoundException("Kategori bulunamadı");

            if (await db.Products.AnyAsync(p => p.CategoryId == id, ct))
                throw new DomainException("Bu kategoride ürün var. Önce ürünleri başka bir kategoriye taşıyın");


            category.MarkDeleted();
            db.Categories.Remove(category);
            await db.SaveChangesAsync(ct);
        }
    }

    internal static class CategoryRules
    {
        public static async Task EnsureNameIsFreeAsync(IAppDbContext db, string name, Guid? exceptId, CancellationToken ct)
        {
            if (await db.Categories.AnyAsync(c => c.Name == name && c.Id != exceptId, ct))
                throw new DomainException($"\"{name}\" adında bir kategori zaten var");
        }
    }
}