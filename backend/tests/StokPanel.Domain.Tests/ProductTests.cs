using StokPanel.Domain.Common;
using StokPanel.Domain.Products;
using StokPanel.Domain.StockMovements;

namespace StokPanel.Domain.Tests;

public class ProductTests
{
    private static Product NewProduct(decimal criticalLevel = 10)
           => Product.Create("Kalem", " klm-001 ", "adet", Guid.NewGuid(), criticalLevel);
    [Fact]
    public void Create_StartsActive_WithZeroStock()
    {
        var product = NewProduct();

        Assert.True(product.IsActive);
        Assert.Equal(0m, product.Quantity);
    }
    [Fact]
    public void Create_NormalizesSku()
    {
        var product = NewProduct();

        Assert.Equal("KLM-001", product.Sku);
    }

    [Fact]
    public void Create_EmptyCategory_Throws()
    {
        Assert.Throws<DomainException>(
            () => Product.Create("Kalem", "KLM-001", "adet", Guid.Empty, 10));
    }
    [Fact]
    public void Create_NegativeCriticalLevel_Throws()
    {
        Assert.Throws<DomainException>(() => NewProduct(criticalLevel: -1));
    }

    [Fact]
    public void SetActive_SameValue_DoesNotRaiseEvent()
    {
        var product = NewProduct();
        product.ClearDomainEvents();

        product.SetActive(true);   // Zaten aktif

        Assert.Empty(product.DomainEvents);
    }
    [Theory]
    [InlineData(5, true)]     // 5 ≤ 10 → kritik
    [InlineData(10, true)]    // Eşit → kritik
    [InlineData(11, false)]   // 11 > 10 → kritik değil
    public void IsCritical_ComparesStockWithCriticalLevel(int stock, bool expected)
    {
        var product = NewProduct(criticalLevel: 10);
        StockMovement.CreateIn(product, stock, "user-1", null);

        Assert.Equal(expected, product.IsCritical);
    }
}
