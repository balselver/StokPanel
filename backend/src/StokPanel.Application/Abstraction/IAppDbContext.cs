using Microsoft.EntityFrameworkCore;
using StokPanel.Domain.Categories;
using StokPanel.Domain.Products;
using StokPanel.Domain.StockMovements;

namespace StokPanel.Application.Abstraction
{
    public interface IAppDbContext
    {
        DbSet<Category> Categories { get; }
        DbSet<Product> Products { get; }
        DbSet<StockMovement> StockMovements { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);


    }
}
