using StokPanel.Domain.Common;
using System.Xml.Linq;

namespace StokPanel.Domain.Categories
{
    public sealed record CategoryCreatedEvent(Guid CategoryId, string Name) : IDomainEvent;
    public sealed record CategoryUpdateEvent(Guid CategoryId, string Name) : IDomainEvent;
    // Silinmeden önce "silindi" notunu bırak (silme EF'in işi, olay Domain'in)
    public sealed record CategoryDeletedEvent(Guid CategoryId, string Name) : IDomainEvent;
}
