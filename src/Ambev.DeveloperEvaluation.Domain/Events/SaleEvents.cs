namespace Ambev.DeveloperEvaluation.Domain.Events.SaleEvents
{
    public record SaleCreatedEvent(Guid Id, object Sale, DateTime CreatedAt) : IEvent;

    public record SaleUpdatedEvent(Guid Id, object Sale, DateTime CreatedAt) : IEvent;

    public record SaleDeletedEvent(Guid Id, DateTime CreatedAt) : IEvent;

    public record SaleDeletedItemEvent(Guid Id, DateTime CreatedAt) : IEvent;
}
