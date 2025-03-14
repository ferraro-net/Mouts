using Ambev.DeveloperEvaluation.Common.Logging;
using Ambev.DeveloperEvaluation.Persistence.Handlers.Sales;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Config;
using Rebus.Handlers;
using Rebus.Serialization.Json;
using Serilog;

namespace Ambev.DeveloperEvaluation.Persistence.RabbitMQ
{
    public static class RebusServiceCollectionExtensions
    {
        public static void AddRebusMessaging(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = configuration.GetSection("RabbitMqSettings").Get<RabbitMqSettings>();

            services.AddRebus(config => config
                    .Logging(l => l.Console())
                    .Transport(t => t.UseRabbitMq(settings.ConnectionString, settings.QueueName).InputQueueOptions(q => q.SetAutoDelete(false)))
                    .Serialization(s => s.UseNewtonsoftJson()));

            var handlers = typeof(SaleCreatedEventHandler).Assembly.GetTypes().Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IHandleMessages<>))).ToList();

            Log.Information("🔍 Rebus Handlers Registrados: {Count}", handlers.Count);
            handlers.ForEach(h => Log.Information("📌 Handler: {HandlerName}", h.Name));

            services.AutoRegisterHandlersFromAssembly(typeof(SaleCreatedEventHandler).Assembly);
        }
    }
}
