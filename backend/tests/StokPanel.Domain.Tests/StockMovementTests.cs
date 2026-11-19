using StokPanel.Domain.Common;
using StokPanel.Domain.Products;
using StokPanel.Domain.StockMovements;
using System;
using System.Collections.Generic;
using System.Text;

namespace StokPanel.Domain.Tests
{

    public class StockMovementTests
    {
        private static Product NewProduct()
            => Product.Create("Kalem", "KLM-001", "adet", Guid.NewGuid(), 10);

        [Fact]
        public void CreateIn_IncreasesStock_AndRecordsQuantityAfter()
        {
            var product = NewProduct();

            var movement = StockMovement.CreateIn(product, 15, "user-1", "Tedarikçiden geldi");

            Assert.Equal(15m, product.Quantity);
            Assert.Equal(StockMovementType.In, movement.Type);
            Assert.Equal(15m, movement.QuantityAfter);
            Assert.Equal(product.Id, movement.ProductId);
        }

        [Fact]
        public void CreateOut_ExactStock_LeavesZero()
        {
            var product = NewProduct();
            StockMovement.CreateIn(product, 5, "user-1", null);

            StockMovement.CreateOut(product, 5, "user-1", null);

            Assert.Equal(0m, product.Quantity);
        }

        [Fact]
        public void CreateOut_MoreThanStock_Throws_AndStockUnchanged()
        {
            var product = NewProduct();
            StockMovement.CreateIn(product, 5, "user-1", null);

            var ex = Assert.Throws<DomainException>(
                () => StockMovement.CreateOut(product, 8, "user-1", null));

            Assert.Contains("Yetersiz stok", ex.Message);
            Assert.Equal(5m, product.Quantity);
        }


        [Theory]
        [InlineData(0)]
        [InlineData(-3)]
        public void CreateIn_ZeroOrNegative_Throws(int quantity)
        {
            var product = NewProduct();

            Assert.Throws<DomainException>(
                () => StockMovement.CreateIn(product, quantity, "user-1", null));
        }

        [Fact]
        public void CreateIn_InactiveProduct_Throws()
        {
            var product = NewProduct();
            product.SetActive(false);

            Assert.Throws<DomainException>(
                () => StockMovement.CreateIn(product, 5, "user-1", null));
        }

        [Fact]
        public void CreateIn_TooLongNote_Throws_AndStockUnchanged()
        {
            var product = NewProduct();
            var longNote = new string('a', StockMovement.NoteMaxLength + 1);

            Assert.Throws<DomainException>(
                () => StockMovement.CreateIn(product, 5, "user-1", longNote));

            // Kontrol sırası doğruysa stok hiç değişmemiş olmalı
            Assert.Equal(0m, product.Quantity);
        }

        [Fact]
        public void CreateIn_EmptyUser_Throws()
        {
            var product = NewProduct();

            Assert.Throws<DomainException>(
                () => StockMovement.CreateIn(product, 5, "", null));
        }

        [Fact]
        public void CreateIn_RaisesStockChangedEvent()
        {
            var product = NewProduct();
            product.ClearDomainEvents();

            StockMovement.CreateIn(product, 5, "user-1", null);

            var domainEvent = Assert.IsType<ProductStockChangedEvent>(Assert.Single(product.DomainEvents));
            Assert.Equal(0m, domainEvent.OldQuantity);
            Assert.Equal(5m, domainEvent.NewQuantity);
        }
    }
}
