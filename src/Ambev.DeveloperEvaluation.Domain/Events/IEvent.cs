namespace Ambev.DeveloperEvaluation.Domain.Events
{
    public interface IEvent
    {
        Guid Id { get; }
        DateTime CreatedAt { get; }
    }
}
