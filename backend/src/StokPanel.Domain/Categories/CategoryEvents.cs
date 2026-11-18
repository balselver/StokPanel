using StokPanel.Domain.Common;

namespace StokPanel.Domain.Categories
{
    public sealed record CategoryCreatedEvent(Guid CategoryId, string Name) : IDomainEvent;
    public sealed record CategoryUpdateEvent(Guid CategoryId, string Name) : IDomainEvent;
}
