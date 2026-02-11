using CarRentalSystem.API.Controllers;
using CarRentalSystem.Infrastructure;
using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Testcontainers.PostgreSql;

namespace CarRentalSystem.IntegrationTests.Helpers;

public class CarRentalSystemApiFactory : WebApplicationFactory<RentalsController>
{
    private readonly PostgreSqlContainer _db;
    
    public CarRentalSystemApiFactory()
    {
        try
        {
            _db = new PostgreSqlBuilder("postgres:16-alpine")
                .WithDatabase(Guid.NewGuid().ToString())
                .WithReuse(false)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("database system is ready to accept connections"))
                .Build();

            _db.StartAsync().Wait();
        }
        catch (Exception ex)
        {
            // Unwrap aggregate exceptions to see the real cause
            if (ex is AggregateException { InnerExceptions.Count: > 0 } agg)
            {
                throw new InvalidOperationException(
                    $"Failed to initialize CarRentalSystemApiFactory: {agg.InnerExceptions[0].Message}", 
                    agg.InnerExceptions[0]);
            }
            
            throw new InvalidOperationException($"Failed to initialize CarRentalSystemApiFactory: {ex.Message}", ex);
        }

    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        Environment.SetEnvironmentVariable("ConnectionStrings__DefaultConnection", _db.GetConnectionString());

        ConfigureLogging(builder);

        builder.ConfigureTestServices(ConfigureDatabase);
    }
    
    private static void ConfigureLogging(IWebHostBuilder builder)
    {
        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Debug);
        });
    }
    
    private void ConfigureDatabase(IServiceCollection services)
    {
        services.RemoveAll<DbContextOptions<CarRentalDbContext>>();
        services.RemoveAll<CarRentalDbContext>();
        var connectionString = _db.GetConnectionString();
        services.AddDbContext<CarRentalDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
            options.EnableDetailedErrors();
        });

        var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CarRentalDbContext>();
        
        dbContext.Database.Migrate();
        
        dbContext.ChangeTracker.Clear();
        
    }
}