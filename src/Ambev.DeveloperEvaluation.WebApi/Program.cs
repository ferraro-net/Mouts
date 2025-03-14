using Ambev.DeveloperEvaluation.Application;
using Ambev.DeveloperEvaluation.Common.HealthChecks;
using Ambev.DeveloperEvaluation.Common.Logging;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.IoC;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.Persistence.RabbitMQ;
using Ambev.DeveloperEvaluation.WebApi.Middleware;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Ambev.DeveloperEvaluation.WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        try
        {
            Log.Information("Starting web application");

            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            var urls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS") ?? "http://localhost:8080";
            Log.Information("ASPNETCORE_URLS: {Url}", urls);
            builder.WebHost.UseUrls(urls);

            builder.Configuration
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Log.Information("Database ConnectionString: {ConnectionString}", builder.Configuration.GetConnectionString("DefaultConnection"));
            Log.Information("MongoDB ConnectionString: {MongoDb}", builder.Configuration.GetSection("MongoDbSettings:ConnectionString").Value);
            Log.Information("RabbitMQ ConnectionString: {RabbitMq}", builder.Configuration.GetSection("RabbitMqSettings:ConnectionString").Value);


            builder.AddDefaultLogging();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.AddBasicHealthChecks();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<DefaultContext>(options =>
                options.UseNpgsql(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("Ambev.DeveloperEvaluation.WebApi")
                )
            );

            builder.Services.AddJwtAuthentication(builder.Configuration);
            builder.RegisterDependencies();
            builder.Services.AddAutoMapper(typeof(Program).Assembly, typeof(ApplicationLayer).Assembly);

            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(
                    typeof(ApplicationLayer).Assembly,
                    typeof(Program).Assembly
                );
            });

            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));
            builder.Services.AddRebusMessaging(builder.Configuration);

            var app = builder.Build();
            app.UseMiddleware<ValidationExceptionMiddleware>();

            // Espera pelo banco de dados estar pronto antes de rodar a migração
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DefaultContext>();

                int retries = 5;
                while (retries > 0)
                {
                    try
                    {
                        Log.Information("Applying database migrations...");
                        dbContext.Database.Migrate();
                        Log.Information("Database migrations applied successfully.");
                        break;
                    }
                    catch (Exception ex)
                    {
                        Log.Warning(ex, "Database migration failed, retrying in 5s...");
                        Thread.Sleep(5000);
                        retries--;
                    }
                }
            }

            //if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseBasicHealthChecks();
            app.MapControllers();

            // Espera RabbitMQ antes de consumir mensagens
            Log.Information("Waiting for RabbitMQ to be ready...");
            Task.Delay(5000).Wait(); // Espera 5s antes de conectar ao RabbitMQ
            app.Services.SubscribeToIntegrationEvents();

            Log.Information("Application started successfully.");
            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
