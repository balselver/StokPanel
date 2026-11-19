using StokPanel.Domain.Common;
using StokPanel.Domain.Products;
using System.Security.Cryptography.X509Certificates;

namespace StokPanel.Domain.StockMovements
{
    public sealed class StockMovement : Entity
    {
        public const int NoteMaxLength = 500;

        public Guid ProductId { get; private set; }
        public StockMovementType Type { get; private set; }
        public decimal Quantity { get; private set; } //hareketteki miktar
        public decimal QuantityAfter { get; private set; }//hareketten sonraki stok
        public string? Note { get; private set; }
        public string UserId { get; private set; } = string.Empty;
        public DateTime CreatedAtUtc { get; private set; }

        private StockMovement()
        {
                
        }

        public static StockMovement CreateIn(Product product, decimal quantity, string userId, string? note)
            => Create(product, StockMovementType.In, quantity, userId, note);

        public static StockMovement CreateOut(Product product, decimal quantity, string userId, string? note)
            => Create(product, StockMovementType.Out, quantity, userId, note);

        private static StockMovement Create(Product product, StockMovementType type, decimal quantity, string userId, string?note)
        {
            ArgumentNullException.ThrowIfNull(product);

            var trimmedNote = string.IsNullOrWhiteSpace(note) ? null : note.Trim();

            if (string.IsNullOrWhiteSpace(userId))
                throw new DomainException("İşlemi yapan kullanıcı bilinmiyor.");
            if (trimmedNote is not null && trimmedNote.Length > NoteMaxLength)
                throw new DomainException($"Not en fazla {NoteMaxLength} karakter olabilir.");

            if (type == StockMovementType.In)
                product.AddStock(quantity);
            else
                product.RemoveStock(quantity);

            return new StockMovement
            {
                ProductId = product.Id,
                Type=type,
                Quantity=quantity,
                QuantityAfter=product.Quantity,
                Note=trimmedNote,
                UserId=userId,
                CreatedAtUtc=DateTime.UtcNow                
            };
        }

    }
}
