using Ambev.DeveloperEvaluation.Common.Logging;
using Ambev.DeveloperEvaluation.Persistence.Handlers.Sales;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Config;
using Rebus.Serialization.Json;

namespace Ambev.DeveloperEvaluation.Persistence.RabbitMQ
{
    public static class RebusServiceCollectionExtensions
    {
        public static void AddRebusMessaging(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = configuration.GetSection("RabbitMqSettings").Get<RabbitMqSettings>();

            services.AddRebus(config => config
                .Transport(t => t.UseRabbitMq(settings.ConnectionString, settings.QueueName).InputQueueOptions(q => q.SetAutoDelete(false)))
                .Serialization(s => s.UseNewtonsoftJson()));

            services.AutoRegisterHandlersFromAssembly(typeof(SaleCreatedEventHandler).Assembly);
        }
    }
}
