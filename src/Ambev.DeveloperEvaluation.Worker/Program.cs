using Ambev.DeveloperEvaluation.Worker;
using Ambev.DeveloperEvaluation.Persistence.RabbitMQ;
using Serilog;
using Ambev.DeveloperEvaluation.Common.Logging;
using Ambev.DeveloperEvaluation.Persistence.MongoDB;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.IoC;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Ambev.DeveloperEvaluation.Application;

try
{
   var host = Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {
            services.Configure<MongoDbSettings>(hostContext.Configuration.GetSection("MongoDbSettings"));
            services.RegisterDependencies();
            services.AddAutoMapper(typeof(Program).Assembly, typeof(ApplicationLayer).Assembly);
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly);
            });
            services.AddRebusMessaging(hostContext.Configuration);
            services.AddDbContext<DefaultContext>(options =>
                options.UseNpgsql(
                    hostContext.Configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("Ambev.DeveloperEvaluation.ORM")
                )
            );

            services.AddHostedService<Worker>();

        }).Build();

    using (var scope = host.Services.CreateScope())
    {
        var provider = scope.ServiceProvider;
        provider.SubscribeToIntegrationEvents();
    }

    MongoDbMappings.RegisterMappings();

    host.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
