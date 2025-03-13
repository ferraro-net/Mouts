namespace Ambev.DeveloperEvaluation.Domain.Events
{
    public interface IEventHandler<T> where T : IEvent
    {
        Task Handle(T message);
    }
}
