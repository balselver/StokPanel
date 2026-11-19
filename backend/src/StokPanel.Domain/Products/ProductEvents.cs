using System;
using System.Collections.Generic;
using System.Text;
using StokPanel.Domain.Common;

namespace StokPanel.Domain.Products
{
    public sealed record ProductCreatedEvent(Guid ProductId, string Name, string Sku) : IDomainEvent;
    public sealed record ProductUpdatedEvent(Guid ProductId, string Name, string Sku) : IDomainEvent;
    public sealed record ProductActiveChangedEvent(Guid ProductId, string Name, bool IsActive) : IDomainEvent;
    public sealed record ProductStockChangedEvent(Guid ProductId, string Name, decimal OldQuantity, decimal NewQuantity) : IDomainEvent;

}
