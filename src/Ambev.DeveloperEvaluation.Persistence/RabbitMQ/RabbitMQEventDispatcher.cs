using Rebus.Bus;
using Ambev.DeveloperEvaluation.Domain.Events;
using Microsoft.Extensions.Configuration;
using Ambev.DeveloperEvaluation.Common.Logging;

namespace Ambev.DeveloperEvaluation.Persistence.RabbitMQ
{
    public class RabbitMQEventDispatcher : IEventDispatcher
    {
        private readonly IBus _bus;
        private readonly IConfiguration _configuration;

        public RabbitMQEventDispatcher(IBus bus, IConfiguration configuration)
        {
            _bus = bus;
            _configuration = configuration;
        }

        public async Task Publish<T>(T @event) where T : IEvent
        {
            var settings = _configuration.GetSection("RabbitMqSettings").Get<RabbitMqSettings>();
            await _bus.Advanced.Routing.Send(settings.QueueName, @event);
            Console.WriteLine($" [x] Published event: {settings.QueueName} -> {@event}");
        }
    }
}
