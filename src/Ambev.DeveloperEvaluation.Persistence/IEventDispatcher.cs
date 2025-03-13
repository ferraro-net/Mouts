using Ambev.DeveloperEvaluation.Domain.Events;

namespace Ambev.DeveloperEvaluation.Persistence
{
    public interface IEventDispatcher
    {
        Task Publish<T>(T @event) where T : IEvent;
    }
}
