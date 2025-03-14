using Ambev.DeveloperEvaluation.Domain.Events.SaleEvents;
using Ambev.DeveloperEvaluation.Domain.Persistence;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using Ambev.DeveloperEvaluation.Persistence;
using Ambev.DeveloperEvaluation.Persistence.Handlers.Sales;
using Ambev.DeveloperEvaluation.Persistence.MongoDB;
using Ambev.DeveloperEvaluation.Persistence.RabbitMQ;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Handlers;

namespace Ambev.DeveloperEvaluation.IoC.ModuleInitializers;

public class InfrastructureModuleInitializer : IModuleInitializer
{
    public void Initialize(IServiceCollection services)
    {
        services.AddScoped<DbContext>(provider => provider.GetRequiredService<DefaultContext>());
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ISaleRepository, SaleRepository>();
        services.AddScoped<IRequestLogService, RequestLogService>();
        services.AddScoped<IEventDispatcher, RabbitMQEventDispatcher>();

        services.AddScoped<IHandleMessages<SaleCreatedEvent>, SaleCreatedEventHandler>();
        services.AddScoped<IHandleMessages<SaleUpdatedEvent>, SaleUpdatedEventHandler>();
        services.AddScoped<IHandleMessages<SaleDeletedEvent>, SaleDeletedEventHandler>();
        services.AddScoped<IHandleMessages<SaleDeletedItemEvent>, SaleDeletedItemEventHandler>();

    }
}