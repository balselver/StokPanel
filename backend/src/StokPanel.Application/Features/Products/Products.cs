using Microsoft.EntityFrameworkCore;
using StokPanel.Application.Abstraction;
using StokPanel.Application.Common;
using StokPanel.Application.Security;
using StokPanel.Domain.Common;
using StokPanel.Domain.Products;

namespace StokPanel.Application.Features.Products
{
    public sealed record ProductDto(
        Guid Id, string Name, string Sku, string Unit, Guid CategoryId, string CategoryName,
        decimal Quantity, decimal CriticalLevel, bool IsCritical, bool IsActive);

    public sealed record ProductFilter(string? Search, Guid? CategoryId, bool? OnlyCritical, bool? IncludeInactive);
    public sealed record SaveProductRequest(string Name, string Sku, string Unit, Guid CategoryId, decimal CriticalLevel);
    public sealed record SetActiveRequest(bool IsActive);

    public sealed class GetProductsHandler(IAppDbContext db, ICurrentUser currentUser)
    {
        public Task<List<ProductDto>> GetProductsHandleAsync(ProductFilter filter, CancellationToken ct)
        {
            var query = db.Products.AsQueryable();

            // Pasif ürünleri sadece admin isterse görür
            var includeInactive = filter.IncludeInactive == true && currentUser.IsInRole(Roles.Admin);
            if (!includeInactive)
                query = query.Where(p => p.IsActive);

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var search = filter.Search.Trim();
                query = query.Where(p => p.Name.Contains(search) || p.Sku.Contains(search));
            }

            if (filter.CategoryId is Guid categoryId)
                query = query.Where(p => p.CategoryId == categoryId);

            if (filter.OnlyCritical == true)
                query = query.Where(p => p.Quantity <= p.CriticalLevel);   // IsCritical DEĞİL (bkz. püf noktası)

            return query.ToDto(db).ToListAsync(ct);
        }
    }

    public sealed class GetProductHandler(IAppDbContext db)
    {
        public async Task<ProductDto> GetProductHandleAsync(Guid id, CancellationToken ct)
            => await db.Products.Where(p => p.Id == id).ToDto(db).FirstOrDefaultAsync(ct)
                ?? throw new NotFoundException("Ürün bulunamadı");
    }

    public sealed class CreateProductHandler(IAppDbContext db)
    {
        public async Task<Guid> CreateProductsHandleAsync(SaveProductRequest r, CancellationToken ct)
        {
            var product = Product.Create(r.Name, r.Sku, r.Unit, r.CategoryId, r.CriticalLevel);
            await ProductRules.EnsureCategoryExistsAsync(db, product.CategoryId, ct);
            await ProductRules.EnsureSkuIsFreeAsync(db, product.Sku, null, ct);

            db.Products.Add(product);
            await db.SaveChangesAsync(ct);
            return product.Id;
        }
    }

    public sealed class UpdateProductHandler(IAppDbContext db)
    {
        public async Task UpdateProductsHandleAsync(Guid id, SaveProductRequest r, CancellationToken ct)
        {
            var product = await db.Products.FirstOrDefaultAsync(p => p.Id == id, ct)
                ?? throw new NotFoundException("Ürün bulunamadı");

            product.Update(r.Name, r.Sku, r.Unit, r.CategoryId, r.CriticalLevel);
            await ProductRules.EnsureCategoryExistsAsync(db, product.CategoryId, ct);
            await ProductRules.EnsureSkuIsFreeAsync(db, product.Sku, product.Id, ct);

            await db.SaveChangesAsync(ct);
        }
    }

    public sealed class SetProductActiveHandler(IAppDbContext db)
    {
        public async Task SetProductHandleAsync(Guid id, SetActiveRequest request, CancellationToken ct)
        {
            var product = await db.Products.FirstOrDefaultAsync(p => p.Id == id, ct)
                ?? throw new NotFoundException("Ürün bulunamadı");

            product.SetActive(request.IsActive);
            await db.SaveChangesAsync(ct);
        }
    }

    internal static class ProductRules
    {
        public static async Task EnsureCategoryExistsAsync(IAppDbContext db, Guid categoryId, CancellationToken ct)
        {
            if (!await db.Categories.AnyAsync(c => c.Id == categoryId, ct))
                throw new DomainException("Seçilen kategori bulunamadı");
        }

        public static async Task EnsureSkuIsFreeAsync(IAppDbContext db, string sku, Guid? exceptId, CancellationToken ct)
        {
            if (await db.Products.AnyAsync(p => p.Sku == sku && p.Id != exceptId, ct))
                throw new DomainException($"{sku} stok kodu başka bir üründe kullanılıyor");
        }
    }

    internal static class ProductQueries
    {
        // Ürün + kategori adı, ada göre sıralı. Liste, tek ürün ve gösterge aynı projeksiyonu kullanır.
        public static IQueryable<ProductDto> ToDto(this IQueryable<Product> products, IAppDbContext db)
            => from p in products
               join c in db.Categories on p.CategoryId equals c.Id
               orderby p.Name
               select new ProductDto(
                   p.Id, p.Name, p.Sku, p.Unit, p.CategoryId, c.Name,
                   p.Quantity, p.CriticalLevel, p.Quantity <= p.CriticalLevel, p.IsActive);
    }
}