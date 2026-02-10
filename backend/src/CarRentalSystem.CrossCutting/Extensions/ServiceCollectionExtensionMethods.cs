using CarRentalSystem.Application.Services;
using CarRentalSystem.Infrastructure.Data;
using CarRentalSystem.Infrastructure.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CarRentalSystem.CrossCutting.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddServices(this IServiceCollection services, IConfiguration config)
    {
        RegisterServicesByConvention(services);
        
    }
    
    public static void AddRepositories(this IServiceCollection services)
    {
        RegisterRepositoriesByConvention(services);
        
    }

    private static void RegisterRepositoriesByConvention(IServiceCollection services)
    {
        services.Scan(scan =>
            scan.FromAssemblyOf<CarCategoryRepository>()
                .AddClasses(classes => classes.Where(c => c.Name.EndsWith("Repository")))
                .AsImplementedInterfaces()
                .AsSelf()
                .WithScopedLifetime());
    }

    
    private static void RegisterServicesByConvention(IServiceCollection services)
    {
        services.Scan(scan =>
            scan.FromAssemblyOf<RentalService>()
                .AddClasses(classes => classes.Where(c => c.Name.EndsWith("Service")))
                .AsImplementedInterfaces()
                .AsSelf()
                .WithScopedLifetime());
    }
    
    public static void AddDbContext(this IServiceCollection services, IConfiguration config)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        services.AddDbContext<CarRentalDbContext>(options =>
        {
            options.UseNpgsql(config.GetConnectionString());
        });
    }

    public static async Task MigrateDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CarRentalDbContext>();
        await context.Database.MigrateAsync();
    }
}