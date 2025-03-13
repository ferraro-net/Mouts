using Ambev.DeveloperEvaluation.IoC.ModuleInitializers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Ambev.DeveloperEvaluation.IoC;

public static class DependencyResolver
{
    public static void RegisterDependencies(this WebApplicationBuilder builder)
    {
        new ApplicationModuleInitializer().Initialize(builder.Services);
        new InfrastructureModuleInitializer().Initialize(builder.Services);
        new WebApiModuleInitializer().Initialize(builder.Services);
    }
    public static void RegisterDependencies(this IServiceCollection services)
    {
        new ApplicationModuleInitializer().Initialize(services);
        new InfrastructureModuleInitializer().Initialize(services);
    }
}