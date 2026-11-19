using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Unicode;
using StokPanel.Domain.Common;
using StokPanel.Domain.Products;

namespace StokPanel.Domain.Products
{
    public sealed class Product : AggregateRoot
    {
        public const int NameMaxLength = 150;
        public const int SkuMaxLength = 50;
        public const int UnitMaxLength = 20;

        public string Name { get; private set; } = string.Empty;
        public string Sku { get; private set; } = string.Empty;
        public string Unit { get; private set; }= string.Empty;

        public Guid CategoryId { get; private set; }
        public decimal Quantity { get; private set; } // mevcut stok
        public decimal CriticalLevel { get; private set; }// bu seviyeye inince uyarı

        public bool IsActive { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }

        public bool IsCritical => Quantity <= CriticalLevel;
        private Product(){ }

        public static Product Create(string name, string sku, string unit, Guid categoryId, decimal criticalLevel)
        {
            var product = new Product { IsActive = true, CreatedAtUtc = DateTime.UtcNow};
            product.SetDetails(name, sku, unit, categoryId, criticalLevel);
            product.Raise(new ProductCreatedEvent(product.Id, product.Name, product.Sku));
            return product;            
        }

        public void Update(string name, string sku, string unit, Guid categoryId, decimal criticalLevel )
        {
            SetDetails(name, sku, unit, categoryId, criticalLevel);
            Raise(new ProductUpdatedEvent(Id, Name, Sku));
        }

        public void SetActive(bool isActive)
        {
            if (IsActive == isActive)
                return;

            IsActive = isActive;
            Raise(new ProductActiveChangedEvent(Id, Name, IsActive));
        }

        internal void AddStock(decimal quantity)
        {
            EnsurePositive(quantity);
            if (!IsActive)
                throw new DomainException("Pasif ürüne stok girişi yapılamaz");

            var oldQuantity = Quantity;
            Quantity += quantity;
            Raise(new ProductStockChangedEvent(Id, Name, oldQuantity, Quantity));
        }

        internal void RemoveStock(decimal quantity)
        {
            EnsurePositive(quantity);
            if (quantity > Quantity)
                throw new DomainException(
                    $"Yetersiz stok. Mevcut {Quantity:0.###} {Unit}, istenen: {quantity:0.###} {Unit}");

            var oldQuantity = Quantity;
            Quantity-= quantity;
            Raise(new ProductStockChangedEvent(Id, Name, oldQuantity, Quantity));
        }


        private static void EnsurePositive(decimal quantity)
        {
            if (quantity <= 0)
                throw new DomainException("Miktar sıfırdan büyük olmalıdır");
        }
        private void SetDetails(string name, string sku, string unit, Guid categoryId, decimal criticalLevel)
        {
            var trimmedName = name?.Trim() ?? string.Empty;
            var normalizedSku = sku?.Trim().ToUpperInvariant() ?? string.Empty;
            var trimmedUnit = unit?.Trim() ?? string.Empty;

            if (trimmedName.Length == 0)
                throw new DomainException("Ürün adı boş olamaz");

            if (trimmedName.Length > NameMaxLength)
                throw new DomainException($"Ürün adı en fazla {NameMaxLength} karakter olabilir.");

            if (normalizedSku.Length == 0)
                throw new DomainException("Stok kodu (SKU) boş olamaz");

            if (normalizedSku.Length > SkuMaxLength)
                throw new DomainException($"Stok kodu en fazla{SkuMaxLength} karakter olabilir.");

            if (trimmedUnit.Length == 0)
                throw new DomainException("Birim boş olamaz");

            if (trimmedUnit.Length > UnitMaxLength)
                throw new DomainException($"Birim en fazla {UnitMaxLength} karakter uzunluuğunda olabilir");

            if (categoryId == Guid.Empty)
                throw new DomainException("Kategori seçilmelidir");

            if (criticalLevel < 0)
                throw new DomainException("Kritik seviye negatif olamaz");

            Name = trimmedName;
            Sku = normalizedSku;
            Unit = trimmedUnit;
            CategoryId = categoryId;
            CriticalLevel = criticalLevel;

        }
    }
}
